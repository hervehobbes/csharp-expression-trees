using System.Globalization;

namespace TitanicExplorer.Data
{
    public class Passenger
    {
        public enum SexValue
        {
            Male = 0,
            Female = 1
        }

        public bool Survived { get; set; }
        public int PClass { get; set; }
        public string Name { get; set; }
        public SexValue Sex { get; set; }
        public decimal Age { get; set; }
        // Frère et soeur ou conjoint
        public int SiblingsOrSpouse { get; set; }
        // Parents ou enfants
        public int ParentOrChildren { get; set; }
        // Tarif
        public decimal Fare { get; set; }

        public static List<Passenger> LoadFromFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            var passengers = new List<Passenger>();

            foreach (var line in lines)
            {
                string[] values = line.Split('\t');

                var passenger = new Passenger
                {
                    Survived = values[0] == "1",
                    PClass = Passenger.ConvertToInt(values[1]),
                    Name = values[2],
                    Sex = values[3] == "male" ? SexValue.Male : SexValue.Female,
                    Age = Passenger.ConvertToInt(values[4]),
                    SiblingsOrSpouse = int.Parse(values[5]),
                    ParentOrChildren = int.Parse(values[6]),
                    Fare = decimal.Parse(values[7], CultureInfo.InvariantCulture)
                };

                passengers.Add(passenger);
            }

            return passengers;
        }

        public static int ConvertToInt(string chaine)
        {
            int nombreEntier = 0;
            try
            {
                //nombreEntier = Convert.ToInt32(double.Parse(chaine));
                nombreEntier = (int)Convert.ToDouble(chaine, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (FormatException)
            {
                Console.WriteLine($"La conversion de {chaine} a échoué. La chaîne n'est pas un nombre valide.");
            }
            return nombreEntier;
        }
    }
}