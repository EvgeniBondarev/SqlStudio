﻿namespace SlqStudio.ViewModel.Log;

public class LogFileListViewModel
{
    public List<LogFileViewModel> Files { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}