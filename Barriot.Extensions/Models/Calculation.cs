﻿namespace Barriot.Extensions.Models
{
    /// <summary>
    ///     A calculation along with its result.
    /// </summary>
    public struct Calculation
    {
        /// <summary>
        ///     The base equation executed over the converter.
        /// </summary>
        public string Equation { get; set; }

        /// <summary>
        ///     The equation result. NaN or default(double) if unsuccesful.
        /// </summary>
        public double Result { get; set; } = double.NaN;

        /// <summary>
        ///     The error of this calculation. If no error occurred, this will be <see cref="string.Empty"/>
        /// </summary>
        public string Error { get; set; } = string.Empty;

        public Calculation(string equation, string errorReason = "")
        {
            Equation = equation;

            if (!string.IsNullOrEmpty(errorReason))
                Error = errorReason;

            try
            {
                var result = new System.Data.DataTable()
                    .Compute(equation, string.Empty);

                if (result == DBNull.Value)
                    Error = "The result of this calculation is not a number.";

                else
                    Result = Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }

        /// <summary>
        ///     Returns a string of the equation result.
        /// </summary>
        /// <returns>
        ///     The equation result.
        /// </returns>
        public override string ToString()
            => $"{Result}";
    }
}
