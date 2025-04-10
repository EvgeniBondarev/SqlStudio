using iTextSharp.text;
using iTextSharp.text.pdf;
using SlqStudio.DTO;
using SlqStudio.Persistence.Models;
using System.IO;
using System.Linq;
using SlqStudio.Application.Services.ReportBuider;

public class PdfReportBuilder : IPdfReportBuilder, IDisposable
{
    private Document _document;
    private MemoryStream _memoryStream;
    private PdfWriter _writer;
    private Font _normalFont;
    private Font _boldFont;
    private Font _headerFont;
    private Font _subHeaderFont;
    private Font _codeFont;
    private BaseColor _successColor;
    private BaseColor _dangerColor;
    private BaseColor _tableHeaderColor;
    private BaseColor _resultBoxColor;

    public PdfReportBuilder()
    {
        _memoryStream = new MemoryStream();
        _document = new Document(PageSize.A4, 40, 40, 40, 40);
        _writer = PdfWriter.GetInstance(_document, _memoryStream);
        
        FontFactory.RegisterDirectories();
        _normalFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 10, Font.NORMAL, BaseColor.BLACK);
        _boldFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 10, Font.BOLD, BaseColor.BLACK);
        _headerFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 16, Font.BOLD, BaseColor.DARK_GRAY);
        _subHeaderFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12, Font.BOLD, BaseColor.DARK_GRAY);
        _codeFont = FontFactory.GetFont("Courier New", BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 8, Font.NORMAL, BaseColor.BLACK);
        
        _successColor = new BaseColor(212, 237, 218);
        _dangerColor = new BaseColor(248, 215, 218);
        _tableHeaderColor = new BaseColor(200, 200, 200);
        _resultBoxColor = new BaseColor(227, 227, 227);
        
        _document.Open();
    }

    public IReportBuilder AddUserInfo(UserDto user)
    {
        // Добавляем дату и время в заголовок
        var dateTime = new Paragraph($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm}", _normalFont);
        dateTime.Alignment = Element.ALIGN_RIGHT;
        _document.Add(dateTime);
        
        var title = new Paragraph("Отчет по тесту", _headerFont);
        title.SpacingAfter = 15f;
        _document.Add(title);
        
        _document.Add(new Paragraph($"Пользователь: {user.Name ?? "Не указан"}", _normalFont));
        _document.Add(new Paragraph($"Email: {user.Email ?? "Не указан"}", _normalFont));
        _document.Add(new Paragraph(" "));
        
        return this;
    }

    public IReportBuilder AddWorkInfo(List<SolutionResultDto> solutions, List<LabWork> labWorks)
    {
        foreach (var lab in labWorks)
        {
            int totalTasks = 0;
            int completedTasks = 0;

            var labTitle = new Paragraph(lab.Name ?? "Без названия", _subHeaderFont);
            labTitle.SpacingAfter = 10f;
            _document.Add(labTitle);
            
            _document.Add(new Paragraph($"Курс: {lab.Course?.Name ?? "Не указан"}", _normalFont));
            _document.Add(new Paragraph(" "));

            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1f, 3f, 4f, 2f });
            
            AddTableHeaderCell(table, "№ Задачи");
            AddTableHeaderCell(table, "Название");
            AddTableHeaderCell(table, "Решение");
            AddTableHeaderCell(table, "Статус");

            foreach (var task in lab.Tasks ?? Enumerable.Empty<LabTask>())
            {
                var solution = solutions?.FirstOrDefault(s => s.TaskId == task.Id);
                bool isSuccess = solution?.IsSuccess == true;
                string status = isSuccess ? "✅ Выполнено" : "❌ Не выполнено";
                string userSolution = solution?.UserSolutionText ?? "Решение отсутствует";
                string shortSolution = userSolution.Length > 50 ? userSolution.Substring(0, 50) + "..." : userSolution;

                var cellColor = isSuccess ? _successColor : _dangerColor;
                
                AddTableCell(table, task.Number.ToString(), cellColor);
                AddTableCell(table, task.Title ?? "Без названия", cellColor);
                AddTableCell(table, shortSolution, cellColor);
                AddTableCell(table, status, cellColor);

                totalTasks++;
                if (isSuccess) completedTasks++;
            }

            _document.Add(table);
            _document.Add(new Paragraph(" "));

            if (totalTasks > 0)
            {
                double score = (double)completedTasks / totalTasks * 10;
                double percentage = (double)completedTasks / totalTasks * 100;

                var resultBox = new PdfPTable(1);
                resultBox.WidthPercentage = 100;
                resultBox.DefaultCell.Border = Rectangle.NO_BORDER;
                resultBox.DefaultCell.BackgroundColor = _resultBoxColor;
                resultBox.DefaultCell.Padding = 10f;
                resultBox.SpacingBefore = 10f;
                resultBox.SpacingAfter = 20f;

                var titleCell = new PdfPCell(new Phrase("Результаты лабораторной", _boldFont));
                titleCell.Border = Rectangle.NO_BORDER;
                resultBox.AddCell(titleCell);

                var resultText = $"Оценка: {score:0.00} из 10,00 ({percentage:0.00}%)";
                var resultCell = new PdfPCell(new Phrase(resultText, _normalFont));
                resultCell.Border = Rectangle.NO_BORDER;
                resultBox.AddCell(resultCell);

                _document.Add(resultBox);
            }
            else
            {
                var noData = new Paragraph("Нет данных для оценки.", _normalFont);
                noData.SpacingAfter = 20f;
                _document.Add(noData);
            }
        }
        
        return this;
    }

    public IReportBuilder AddSolutionDetails(List<SolutionResultDto> solutions, List<LabWork> labWorks)
    {
        var detailsTitle = new Paragraph("Детали решений", _subHeaderFont);
        detailsTitle.SpacingAfter = 10f;
        _document.Add(detailsTitle);

        foreach (var solution in solutions ?? Enumerable.Empty<SolutionResultDto>())
        {
            var lab = labWorks?.FirstOrDefault(l => l.Tasks?.Any(t => t.Id == solution.TaskId) == true);
            var task = lab?.Tasks?.FirstOrDefault(t => t.Id == solution.TaskId);

            var taskTitleText = $"{lab?.Name ?? "Неизвестная лабораторная"} - {task?.Title ?? "Неизвестная задача"}";
            var taskTitle = new Paragraph(taskTitleText, _boldFont);
            taskTitle.SpacingAfter = 5f;
            _document.Add(taskTitle);

            string solutionText = solution.UserSolutionText ?? "Решение отсутствует";
            
            var codeBlock = new PdfPTable(1);
            codeBlock.WidthPercentage = 100;
            codeBlock.DefaultCell.Border = Rectangle.NO_BORDER;
            codeBlock.DefaultCell.BackgroundColor = new BaseColor(39, 40, 34);
            codeBlock.DefaultCell.Padding = 10f;
            codeBlock.SpacingAfter = 15f;

            var codeCell = new PdfPCell(new Phrase(solutionText, _codeFont));
            codeCell.Border = Rectangle.NO_BORDER;
            codeCell.HorizontalAlignment = Element.ALIGN_LEFT;
            codeBlock.AddCell(codeCell);

            _document.Add(codeBlock);
        }
        
        return this;
    }

    private void AddTableHeaderCell(PdfPTable table, string text)
    {
        var cell = new PdfPCell(new Phrase(text, _boldFont));
        cell.BackgroundColor = _tableHeaderColor;
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.Padding = 5f;
        cell.BorderColor = BaseColor.WHITE;
        table.AddCell(cell);
    }

    private void AddTableCell(PdfPTable table, string text, BaseColor backgroundColor)
    {
        var cell = new PdfPCell(new Phrase(text, _normalFont));
        cell.BackgroundColor = backgroundColor;
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.Padding = 5f;
        table.AddCell(cell);
    }

    public byte[] Build()
    {
        try
        {
            _document.Close();
            return _memoryStream.ToArray();
        }
        finally
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        _writer?.Close();
        _writer?.Dispose();
        _memoryStream?.Dispose();
        _document?.Dispose();
    }
}