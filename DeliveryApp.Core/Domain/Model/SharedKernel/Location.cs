using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json.Linq;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.SharedKernel
{
    /// <summary>
    ///     координата
    /// </summary>
    public class Location : ValueObject
    {
        #region PROPERTIES
        /// <summary>
        ///     Константы 
        /// </summary>
        private const int minPosition = 1;
        private const int maxPosition = 10;


        /// <summary>
        ///     Координата по горизонтали
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        ///     Координата по вертикали
        /// </summary>
        public int Y { get; private set; }
        #endregion

        /// <summary>
        ///     Ctr
        /// </summary>
        private Location()
        {
        }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="x">Координата по горизонтали</param>
        /// <param name="y">Координата по вертикали</param>
        private Location(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Factory Method
        /// </summary>
        /// <param name="x">Координата по горизонтали</param>
        /// <param name="y">Координата по вертикали</param>
        /// <returns>Результат</returns>
        public static Result<Location, Error> Create(int x, int y)
        {
            if (x < minPosition || x > maxPosition) return GeneralErrors.ValueIsRequired(nameof(x));
            if (y < minPosition || y > maxPosition) return GeneralErrors.ValueIsRequired(nameof(x));

            return new Location(x,y);
        }

        /// <summary>
        ///     Создание рандомной координаты
        /// </summary>
        /// <returns>Новые рандомные координаты</returns>
        public static Location CreateRandom()
        {
            var randomX = new Random().Next(minPosition, maxPosition);
            var randomY = new Random().Next(minPosition, maxPosition);

            return new Location(randomX, randomY);
        }

        /// <summary>
        ///     Расчет расстояние между двумя координатами
        /// </summary>
        /// <param name="target">Координата для рассчета</param>
        /// <returns>Результат</returns>
        public Result<int, Error> DistanceTo(Location target)
        {
            if (target is null) return GeneralErrors.ValueIsRequired(nameof(target));

            var deltaX = Math.Abs(target.X - X);
            var deltaY = Math.Abs(target.Y - Y);

            var distance = deltaX + deltaY;

            return distance;
        }

        /// <summary>
        ///     Перегрузка для определения идентичности
        /// </summary>
        /// <returns>Результат</returns>
        /// <remarks>Идентичность будет происходить по совокупности полей указанных в методе</remarks>
        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }
}
