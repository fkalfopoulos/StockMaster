using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FantasyWealth.Models
{
      public enum TransactionType{
        Transfer,
        Debit,
        Credit
    }
    public enum TradeAction{
        Sell,
        Buy
    }
    public enum TradeStatus{
        Pending,
        Processing,
        Approved,
        Cancelled,
        Disabled
    }
      public enum Account
    {
        Bank,
        Broker,
    }
}