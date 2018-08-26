using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FitnessTrackerPlus.Views.Measurement
{
    public partial class MeasurementLog : NavigationPage
    {
        private MeasurementContext context = new MeasurementContext();

        public MeasurementLog()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                // Setup control event handlers

                Calendar.SelectedDate = Globals.SelectedDate;
                Calendar.SelectedDatesChanged += (se, ev) =>
                {
                    if (Calendar.SelectedDate.HasValue)
                    {
                        Globals.SelectedDate = Calendar.SelectedDate.Value;

                        LoadCurrentImage();
                    }
                };

                MeasurementLogGrid.LayoutUpdated += (se, ev) => { DataGridHelper.ResizeGrid(0, MeasurementLogGrid); };
                MeasurementLogGrid.RowEditEnded += new EventHandler<DataGridRowEditEndedEventArgs>(MeasurementLogGrid_RowEditEnded);
                MeasurementLogGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(MeasurementLogGrid_LoadingRow);

                CustomMeasurement.Click += new RoutedEventHandler(CustomMeasurement_Click);
                UpdateImage.Click += new RoutedEventHandler(UpdateImage_Click);
                DeleteSelected.Click += new RoutedEventHandler(DeleteSelected_Click);

                // Hold onto a copy of the MeasurementContext contained in the XAML

                context = this.Resources["MeasurementContext"] as MeasurementContext;

                Parameter user_id = new Parameter();
                user_id.ParameterName = "user_id";
                user_id.Value = Globals.CurrentUser.id;

                // Ensure that the user_id parameter is set for the DomainDataSource control before the query is executed

                MeasurementData.QueryParameters.Add(user_id);

                // Ensure that whenever data is being loaded into the DomainContext the ProgressBar control is visible
                // The ProgressBarVisibilityConverter will ensure that when IsLoading is true the Visibility is set to visible, false otherwise

                Binding binding = new Binding();
                binding.Source = context;
                binding.Path = new PropertyPath("IsLoading");
                binding.Converter = new ProgressBarVisibilityConverter();

                ProgressBar.SetBinding(ProgressBar.VisibilityProperty, binding);

                // Setup the ComboBox control now that the MeasurementContext is available

                context.Load<FitnessTrackerPlus.Web.Data.Measurement>(context.GetMeasurementsQuery(Globals.CurrentUser.id),
                    LoadBehavior.RefreshCurrent, (MeasurementsLoaded) =>
                    {
                        if (!MeasurementsLoaded.HasError)
                        {
                            Measurements.ItemsSource = MeasurementsLoaded.Entities;
                            Measurements.SelectedIndex = 0;
                        }

                    }, null);

                Measurements.SelectionChanged += (se, ev) =>
                {
                    FitnessTrackerPlus.Web.Data.Measurement selected = Measurements.SelectedItem as FitnessTrackerPlus.Web.Data.Measurement;

                    if (selected.id > 0)
                        CreateMeasurementLogEntry(selected);
                };

                LoadCurrentImage();
            };
        }

        #region Control Event Handlers

        private void CheckAll_Checked(object sender, RoutedEventArgs e)
        {
            DataGrid grid = DataGridHelper.GetParentGrid(sender as CheckBox);

            foreach (MeasurementLogEntry entry in grid.ItemsSource)
            {
                grid.SelectedItem = entry;
                CheckBox selectItem = grid.Columns[grid.Columns.Count - 1].GetCellContent(grid.SelectedItem) as CheckBox;

                if (selectItem != null)
                    selectItem.IsChecked = (sender as CheckBox).IsChecked;
            }
        }

        private void MeasurementLogGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            MeasurementLogEntry entry = e.Row.DataContext as MeasurementLogEntry;

            // If this entry has a valid calculator associated then change the TextBlock to a HyperlinkButton
            // that when clicked will use the Acivator object to dynamically create the appropriate calculator

            if (entry.Measurement.Calculator != null)
            {
                TextBlock measurementName = MeasurementLogGrid.Columns[0].GetCellContent(e.Row) as TextBlock;

                if (measurementName != null)
                {
                    DataGridCell cell = measurementName.Parent as DataGridCell;
                    HyperlinkButton calculatorLink = new HyperlinkButton();

                    calculatorLink.Content = entry.Measurement.measurement_name;
                    calculatorLink.Margin = new Thickness(5);
                    calculatorLink.HorizontalAlignment = HorizontalAlignment.Center;
                    calculatorLink.Click += (s, ev) =>
                    {
                        // Create an instance of the calculator using IMeasurementCalculator interface so that we have
                        // access to the calculated value and can update the selected row value

                        ChildWindow modalWindow = new ChildWindow();
                        IMeasurementCalculator calc = Activator.CreateInstance(Type.GetType(String.Format("FitnessTrackerPlus.Views.Measurement.Calculators.{0}",
                            entry.Measurement.Calculator.type_name))) as IMeasurementCalculator;

                        calc.CalculationCancelled += (se, eve) => { modalWindow.Close(); };
                        calc.CalculationComplete += (se, eve) =>
                        {
                            modalWindow.Close();

                            entry.value = calc.CalculatedValue;
                            context.SubmitChanges();
                        };

                        modalWindow.Title = String.Format("{0} Calculator", entry.Measurement.measurement_name);
                        modalWindow.Content = calc;
                        modalWindow.Show();
                    };

                    // Repace the TextBlock with the HyperlinkButton control

                    cell.Content = calculatorLink;
                }
            }
        }

        private void UpdateImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Multiselect = false;
            dialog.Filter = "Supported Images (*.png, *.jpg)|*.png;*.jpg|PNG Images (*.png)|*.png|JPG Images (*.jpg)|*.jpg";

            bool? result = dialog.ShowDialog();

            if (result != null && result == true)
            {
                WebClient client = new WebClient();
                string finalFileName = "";

                // Open up a connection to the ashx handler for image uploading
                // once connected open the image file and write the raw bytes to the 
                // provided http stream

                client.OpenWriteCompleted += (s, ev) =>
                {
                    if (ev.Error == null)
                    {
                        FileStream file_stream = dialog.File.OpenRead();
                        byte[] image_buffer = new byte[4096];
                        int bytes_read = 0;

                        if (file_stream != null)
                        {
                            while ((bytes_read = file_stream.Read(image_buffer, 0, image_buffer.Length)) != 0)
                                ev.Result.Write(image_buffer, 0, bytes_read);

                            file_stream.Close();
                            ev.Result.Close();

                            // Once the image is uploaded add an entry to the measurements_images table

                            context.MeasurementImages.Add(new MeasurementImage
                            {
                                entry_date = Globals.SelectedDate,
                                user_id = Globals.CurrentUser.id,
                                file_name = finalFileName
                            });

                            context.SubmitChanges();

                            // Finally display the newly uploaded image

                            BitmapImage updatedImage = null;
#if DEBUG
                            updatedImage = new BitmapImage(new Uri(String.Format("http://localhost:22403/UploadedImages/{0}",
                                finalFileName), UriKind.Absolute));

#else
                                updatedImage = new BitmapImage(new Uri(String.Format("http://fitnesstrackerplus.com/UploadedImages/{0}",
                                    finalFileName), UriKind.Absolute));
#endif

                            // This is necessary to ensure that Silverlight refreshes the image even though the file name remains the same

                            updatedImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            CurrentImage.Source = updatedImage;
                        }
                    }
                };

                // Don't allow images larger than 500k

                if (dialog.File.Length > 512000)
                    MessageBox.Show("Only images up to 500k are supported");
                else
                {
                    // Convert the file name to one that matches the convention
                    // from the design user_id_entry_date_measurement_image

                    finalFileName = String.Format("{0}_{1}_measurement_image{2}",
                        Globals.CurrentUser.id, Globals.SelectedDate.ToString("MM_dd_yyyy"),
                        dialog.File.Extension);

                    client.OpenWriteAsync(new Uri(String.Format("http://localhost:22403/ImageUpload.ashx?file_name={0}",
                        finalFileName), UriKind.Absolute));
                }
            }
        }

        private void CustomMeasurement_Click(object sender, RoutedEventArgs e)
        {
            // Show a modal dialog with the create custom measurement form

            ChildWindow modalWindow = new ChildWindow();
            CustomMeasurement customMeasurement = new CustomMeasurement();

            customMeasurement.CustomMeasurementCancelled += (s, ev) => { modalWindow.Close(); };
            customMeasurement.CustomMeasurementCreated += (s, ev) =>
            {
                CreateMeasurementLogEntry(ev.CreatedMeasurement);
                modalWindow.Close();
            };

            modalWindow.Title = "Add Custom Measurement";
            modalWindow.Content = customMeasurement;
            modalWindow.Show();
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            List<MeasurementLogEntry> entries = new List<MeasurementLogEntry>();
            ProgressBar.Visibility = Visibility.Visible;

            foreach (MeasurementLogEntry entry in context.MeasurementLogEntries)
            {
                CheckBox selectItem = null;

                MeasurementLogGrid.SelectedItem = entry;
                selectItem = MeasurementLogGrid.Columns[MeasurementLogGrid.Columns.Count - 1].GetCellContent(MeasurementLogGrid.SelectedItem) as CheckBox;

                if (selectItem != null)
                    if (selectItem.IsChecked == true)
                        entries.Add(entry);
            }

            foreach (MeasurementLogEntry entry in entries)
                context.MeasurementLogEntries.Remove(entry);

            context.SubmitChanges((EntriesRemoved) =>
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                MeasurementData.Load();

            }, null);
        }

        private void MeasurementLogGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            // Submit any measurement log changes and refresh the DataGrid

            if (context.HasChanges)
                context.SubmitChanges();
        }

        #endregion

        #region Private Methods

        private void CreateMeasurementLogEntry(FitnessTrackerPlus.Web.Data.Measurement measurement)
        {
            MeasurementLogEntry entry = new MeasurementLogEntry();

            // Setup new measurement log entry with selected date, measurement and current user

            IEnumerator units = measurement.MeasurementsUnits.GetEnumerator();
            units.MoveNext();

            entry.measurement_id = measurement.id;
            entry.entry_date = Globals.SelectedDate;
            entry.user_id = Globals.CurrentUser.id;
            entry.unit_id = (units.Current as MeasurementsUnits).unit_id;

            context.MeasurementLogEntries.Add(entry);
            context.SubmitChanges((ChangesSubmitted) =>
            {
                if (!ChangesSubmitted.HasError)
                    MeasurementData.Load();

            }, null);
        }

        private void LoadCurrentImage()
        {
            context.Load<MeasurementImage>(context.GetMeasurementImageQuery(Globals.CurrentUser.id, Globals.SelectedDate),
                LoadBehavior.RefreshCurrent, (ImageLoaded) =>
                    {
                        if (!ImageLoaded.HasError)
                        {
                            IEnumerator<MeasurementImage> enumerator = ImageLoaded.Entities.GetEnumerator();
                            enumerator.MoveNext();

                            MeasurementImage image = enumerator.Current;

                            if (image != null)
                            {
                                BitmapImage updatedImage = null;
#if DEBUG
                                updatedImage = new BitmapImage(new Uri(String.Format("http://localhost:22403/UploadedImages/{0}",
                                    image.file_name), UriKind.Absolute));
#else
                                updatedImage = new BitmapImage(new Uri(String.Format("http://fitnesstrackerplus.com/UploadedImages/{0}",
                                    image.file_name), UriKind.Absolute));
#endif

                                updatedImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                                CurrentImage.Source = updatedImage;
                            }
                            else
                                CurrentImage.Source = new BitmapImage(new Uri("/Images/image_unavailable.png", UriKind.Relative));
                        }
                        else
                            CurrentImage.Source = new BitmapImage(new Uri("/Images/image_unavailable.png", UriKind.Relative));

                    }, null);
        }

        #endregion
    }
}
