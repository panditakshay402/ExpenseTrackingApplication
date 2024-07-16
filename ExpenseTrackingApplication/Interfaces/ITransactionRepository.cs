using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;

namespace ExpenseTrackingApplication.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction?>> GetAll();
    Task<Transaction?> GetById(int id);
    Task<IEnumerable<Transaction>> GetTransactionByCategory(TransactionCategory category);
    bool Add(Transaction transaction);
    bool Delete(Transaction transaction);
    bool Update(Transaction transaction);
    bool Save();
}