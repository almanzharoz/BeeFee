using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFee.Model.Embed
{
    public enum ETransactionType : short
    {
		Unknow = 0,
		/// <summary>
		/// Бесплатная регистрация
		/// </summary>
		Registrition,
		/// <summary>
		/// Покупка билета с оплатой
		/// </summary>
		Ticket,

    }
}
