namespace Wallet.Domain.Exceptions;

public class BalanceNegativeException(string message) : Exception(message);