using System;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Exceptions
{
    public class AddEntityException<T> : Exception where T : IModel
    {
        public AddEntityException() : base($"Не удалось добавить \"{typeof(T).Name}\" в БД")
        {
        }

        public AddEntityException(string message) : base(message)
        {
        }

        public AddEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}