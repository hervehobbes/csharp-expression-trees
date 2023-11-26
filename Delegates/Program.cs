using System.Linq.Expressions;

namespace Delegates
{
    internal class Program
    {
        static void Main()
        {
            // Construction de notre Expression Tree (x > 12)
            // On a un paramètre, x, de type int
            var xExpression = Expression.Parameter(typeof(int), "x");
            // On se déplace de la gauche vers la droite et on construit > 12
            var constantExpression = Expression.Constant(12);
            var greatorThan = Expression.GreaterThan(xExpression, constantExpression);

            var constant4Expression = Expression.Constant(4);
            var lessThan = Expression.LessThan(xExpression, constant4Expression);

            // On ajoute un `or` x < 2 OU x > 12
            var or = Expression.Or(greatorThan, lessThan);

            // Maintenant on a besoin de prendre cette expression et de la compiler en une fonction qu'on peut exécuter
            // C'est le corps complet d'une fonction
            // Le deuxième argument, false, est un détail d'optimisation
            // Le dernier argument représente les paramètres
            // var expr = Expression.Lambda<Func<int, bool>>(greatorThan, false, new List<ParameterExpression> { xExpression });
            // Ajout du or
            var expr = Expression.Lambda<Func<int, bool>>(or, false, new List<ParameterExpression> { xExpression });
            // Fonction exécutable
            var func = expr.Compile();
            Console.WriteLine(func(2));



            var guitars = new List<Guitar>
            {
                new Guitar(PickupType.Electric, StringType.Steel, "Cherry Red Strat"),
                new Guitar(PickupType.AcousticElectric, StringType.Nylon, "Takamine EG-116"),
                new Guitar(PickupType.Acoustic, StringType.Steel, "Martin D-X1E")
            };

            Func<Guitar, bool> nylon = guitar => guitar.Strings == StringType.Nylon;
            var nylonGuitars = guitars.Where(nylon);
            //foreach (var item in nylonGuitars)
            //{
            //    Console.WriteLine($"{item.Name}");
            //}

            // IEnumerable<Guitar> electricGuitars = guitars.Where(guitar => guitar.Pickup == PickupType.Electric);
            //IEnumerable<Guitar> electricGuitars = Enumerable.Where<Guitar>(guitars, guitar => guitar.Pickup == PickupType.Electric);
            //foreach (var item in electricGuitars)
            //{
            //    Console.WriteLine($"{item.Name}");
            //}

            //var xExpression = Expression.Parameter(typeof(int), "x");
            //var constantExpression = Expression.Constant(12);
            //var greaterThan = Expression.GreaterThan(xExpression, constantExpression);

            //var constant4Expression = Expression.Constant(4);
            //var lessThan = Expression.LessThan(xExpression, constant4Expression);

            //var or = Expression.Or(greaterThan, lessThan);

            //var expr = Expression.Lambda<Func<int, bool>>(or, false, new List<ParameterExpression> { xExpression, });
            //var func = expr.Compile();

            //Console.WriteLine(func(2));
        }
    }
}