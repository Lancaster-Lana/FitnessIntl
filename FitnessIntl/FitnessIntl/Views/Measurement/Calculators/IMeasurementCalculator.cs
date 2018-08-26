using System;

public interface IMeasurementCalculator
{
    event EventHandler CalculationComplete;
    event EventHandler CalculationCancelled;

    double CalculatedValue { get; set; }
}