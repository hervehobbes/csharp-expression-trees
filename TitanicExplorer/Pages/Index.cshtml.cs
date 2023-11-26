namespace TitanicExplorer.Pages
{
    using AgileObjects.ReadableExpressions;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.IO;
    using System.Linq.Dynamic.Core;
    using System.Linq.Expressions;
    using TitanicExplorer.Data;
    using static TitanicExplorer.Data.Passenger;

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

            var sampleDataPath = Path.GetTempFileName();

            System.IO.File.WriteAllText(sampleDataPath, DataFiles.passengers);

            this.Passengers = Passenger.LoadFromFile(sampleDataPath);
        }

        public IEnumerable<Passenger> Passengers
        {
            get; private set;
        }

        public void OnGet()
        {

        }

        public string query { get; set; }

        public void OnPost()
        {
            var survived = Request.Form["survived"] != "" ? ParseSurvived(Request.Form["survived"]) : null;
            var pClass = ParseNullInt(Request.Form["pClass"]);
            var sex = Request.Form["sex"] != "" ? ParseSex(Request.Form["sex"]) : null;
            var age = ParseNullDecimal(Request.Form["age"]);
            var minimumFare = ParseNullDecimal(Request.Form["minimumFare"]);
            this.query = Request.Form["query"];

            this.Passengers = FilterPassengers(survived, pClass, sex, age, minimumFare);
        }

        private IEnumerable<Passenger> FilterPassengers(bool? survived, int? pClass, SexValue? sex, decimal? age, decimal? minimumFare)
        {
            /*
            Expression currentExpression = null;
            // Le paramètre de la fonction qui va être construite
            var passengerParameter = Expression.Parameter(typeof(Passenger), "passenger");
            // Autre forme possible :
            //var passengerParameter = Expression.Parameter(typeof(Passenger));

            if (survived is not null)
            {
                // Il faut la partie gauche et l'expression, `Survived`, l'opérateur `=` et la valeur avec laquelle comparer `True` ou `False`
                // La valeur a comparer (la partie droite), True ou False
                var survivedValue = Expression.Constant(survived.Value);
                // Le champ, la propriété de la classe avec laquelle on fait la comparaison
                // On a besoin de comparer la valeur de la propriété, la propriété `Survived`
                // depuis mon paramètre (objet) `passenger`, je récupère la propriété `Survived`
                var passengerSurvived = Expression.Property(passengerParameter, "Survived");
                // (paramètre) Passenger.Survived(propriété) = survivedValue (valeur reçue de l'utilisateur)
                // La propriété de la classe à utliser pour comparer et la valeur de comparaison
                //var survivedEquals = Expression.Equal(passengerSurvived, survivedValue);
                currentExpression = Expression.Equal(passengerSurvived, survivedValue);
                // Et on crée l'expression avec la lambda
                //var expr = Expression.Lambda<Func<Passenger, bool>>(survivedEquals, false, new List<ParameterExpression> { passengerParameter });
                // On compile l'expression en une fonction
                //var func = expr.Compile();
                // Ancien code :
                //this.Passengers = this.Passengers.Where(passenger => passenger.Survived == survived);
                //this.Passengers = this.Passengers.Where(func);
            }

            if (pClass is not null)
            {
                // La valeur à comparer, la partie droite
                var pClassValue = Expression.Constant(pClass.Value);
                // La propriété de notre paramètre sur laquelle portera le test
                var passengerClassValue = Expression.Property(passengerParameter, "PClass");
                var pClassEquals = Expression.Equal(passengerClassValue, pClassValue);
                if (currentExpression is null)
                {
                    currentExpression = pClassEquals;
                }
                else
                {
                    var previousExpression = currentExpression;
                    currentExpression = Expression.And(previousExpression, pClassEquals);
                }
                //var pClassExpr = Expression.Lambda<Func<Passenger, bool>>(pClassEquals, false, new List<ParameterExpression> { passengerParameter });
                //var funcpClassExpr = pClassExpr.Compile();
                //this.Passengers = this.Passengers.Where(passenger => passenger.PClass == pClass);
                //this.Passengers = this.Passengers.Where(funcpClassExpr);
            }

            if (sex is not null)
            {
                // La valeur à comparer, la partie droite
                var pSexValueToCompare = Expression.Constant(sex.Value);
                // La propriété de notre paramètre sur laquelle portera le test
                var passengerSexProperty = Expression.Property(passengerParameter, "Sex");
                // Le lien entre la propriété et la valeur de comparaison
                var pSexEquals = Expression.Equal(passengerSexProperty, pSexValueToCompare);
                if (currentExpression is null)
                {
                    currentExpression = pSexEquals;
                }
                else
                {
                    var previousExpression = currentExpression;
                    currentExpression = Expression.And(previousExpression, pSexEquals);
                }

                //var pSexExpr = Expression.Lambda<Func<Passenger, bool>>(pSexEquals, false, new List<ParameterExpression> { passengerParameter });
                //var funcSexExpr = pSexExpr.Compile();

                //this.Passengers = this.Passengers.Where(passenger => passenger.Sex == sex);
                //this.Passengers = this.Passengers.Where(funcSexExpr);
            }

            if (age is not null)
            {
                var pAgeValueToCompare = Expression.Constant(age.Value);
                // La propriété de notre paramètre sur laquelle portera le test
                var passengerAgeProperty = Expression.Property(passengerParameter, "Age");
                // Le lien entre la propriété et la valeur de comparaison
                var pAgeEquals = Expression.Equal(passengerAgeProperty, pAgeValueToCompare);
                if (currentExpression is null)
                {
                    currentExpression = pAgeEquals;
                }
                else
                {
                    var previousExpression = currentExpression;
                    currentExpression = Expression.And(previousExpression, pAgeEquals);
                }

                //var pAgeExpr = Expression.Lambda<Func<Passenger, bool>>(pAgeEquals, false, new List<ParameterExpression> { passengerParameter });
                //var funcAgeExpr = pAgeExpr.Compile();

                //this.Passengers = this.Passengers.Where(passenger => passenger.Age == age);
                //this.Passengers = this.Passengers.Where(funcAgeExpr);
            }

            if (minimumFare is not null)
            {
                var pFareValueToCompare = Expression.Constant(minimumFare.Value);
                // La propriété de notre paramètre sur laquelle portera le test
                var passengerFareProperty = Expression.Property(passengerParameter, "Fare");
                // Le lien entre la propriété et la valeur de comparaison
                var pFareGreaterOrEqual = Expression.GreaterThanOrEqual(passengerFareProperty, pFareValueToCompare);
                if (currentExpression is null)
                {
                    currentExpression = pFareGreaterOrEqual;
                }
                else
                {
                    var previousExpression = currentExpression;
                    currentExpression = Expression.And(previousExpression, pFareGreaterOrEqual);
                }

                //var pFareExpr = Expression.Lambda<Func<Passenger, bool>>(pFareGreaterOrEqual, false, new List<ParameterExpression> { passengerParameter });
                //var funcFareExpr = pFareExpr.Compile();

                //this.Passengers = this.Passengers.Where(passenger => passenger.Fare >= minimumFare);
                //this.Passengers = this.Passengers.Where(funcFareExpr);
            }

            if (currentExpression is not null)
            {
                var globalLambdaExpr = Expression.Lambda<Func<Passenger, bool>>(currentExpression, false, new List<ParameterExpression> { passengerParameter });
                var globalLambdaExprCompiled = globalLambdaExpr.Compile();
                this.Passengers = this.Passengers.Where(globalLambdaExprCompiled);
            }

            return this.Passengers;

            */

            Expression? currentExpression = null;
            //var passengerParameter = Expression.Parameter(typeof(Passenger), "passenger");

            if (!string.IsNullOrEmpty(this.query))
            {
                var expr = DynamicExpressionParser.ParseLambda<Passenger, bool>(new ParsingConfig(), true, this.query);

                var func = expr.Compile();

                return this.Passengers.Where(func);
            }

            var passengerParameter = Expression.Parameter(typeof(Passenger));

            if (survived != null)
            {
                currentExpression = CreateExpression<bool>(survived.Value, null, "Survived", passengerParameter);
            }

            if (pClass != null)
            {
                currentExpression = CreateExpression<int>(pClass.Value, currentExpression, "PClass", passengerParameter);
            }

            if (sex != null)
            {
                currentExpression = CreateExpression<SexValue>(sex.Value, currentExpression, "Sex", passengerParameter);
            }

            if (age != null)
            {
                currentExpression = CreateExpression<decimal>(age.Value, currentExpression, "Age", passengerParameter);
            }

            if (minimumFare != null)
            {
                currentExpression = CreateExpression<decimal>(minimumFare.Value, currentExpression, "Fare", passengerParameter, ">");
            }

            if (currentExpression != null)
            {
                var expr = Expression.Lambda<Func<Passenger, bool>>(currentExpression, false, new List<ParameterExpression> { passengerParameter });
                var func = expr.Compile();

                this.query = expr.ToReadableString();

                this.Passengers = this.Passengers.Where(func);
            }

            return this.Passengers;
        }

        /// <summary>
        /// Aggregates an expression with a property and an operator
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <param name="value">The constant value to use in the expression</param>
        /// <param name="currentExpression">The expression to aggregate with, if any</param>
        /// <param name="propertyName">The name of the property to call on the objectParameter</param>
        /// <param name="objectParameter">The parameter for the object for evaluation</param>
        /// <param name="operatorType">A string of the operator to use</param>
        /// <returns></returns>
        private static Expression CreateExpression<T>(T value, Expression? currentExpression, string propertyName, ParameterExpression objectParameter, string operatorType = "=")
        {
            var valueToTest = Expression.Constant(value);

            var propertyToCall = Expression.Property(objectParameter, propertyName);

            Expression operatorExpression;

            switch (operatorType)
            {
                case ">":
                    operatorExpression = Expression.GreaterThan(propertyToCall, valueToTest);
                    break;
                case "<":
                    operatorExpression = Expression.LessThan(propertyToCall, valueToTest);
                    break;
                case ">=":
                    operatorExpression = Expression.GreaterThanOrEqual(propertyToCall, valueToTest);
                    break;
                case "<=":
                    operatorExpression = Expression.LessThanOrEqual(propertyToCall, valueToTest);
                    break;
                default:
                    operatorExpression = Expression.Equal(propertyToCall, valueToTest);
                    break;
            }

            if (currentExpression == null)
            {
                currentExpression = operatorExpression;
            }
            else
            {
                var previousExpression = currentExpression;

                currentExpression = Expression.And(previousExpression, operatorExpression);
            }

            return currentExpression;
        }

        public decimal? ParseNullDecimal(string value)
        {
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }

            return null;
        }

        public int? ParseNullInt(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            return null;
        }

        public SexValue? ParseSex(string value)
        {
            return value == "male" ? SexValue.Male : SexValue.Female;
        }

        public bool? ParseSurvived(string value)
        {
            return value == "Survived" ? true : false;
        }
    }
}