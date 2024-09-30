using ExpenseTrackingApplication.Interfaces;
using ExpenseTrackingApplication.ViewModels;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using ExpenseTrackingApplication.Data;
using ExpenseTrackingApplication.Data.Enum;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }
    
}