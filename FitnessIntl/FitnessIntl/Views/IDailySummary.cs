using System;

public interface IDailySummary
{
    void LoadSummary(int user_id, DateTime summary_date);
}