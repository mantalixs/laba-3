using System;
using System.IO;
using Newtonsoft.Json;

class PolynomialJsonRepresentation
{
    public int Degree { get; set; }
    public double[] Coefficients { get; set; }

    public PolynomialJsonRepresentation(int degree, double[] coefficients)
    {
        Degree = degree;
        Coefficients = coefficients;
    }
}

class NewPolynomial
{
    public int Degree { get; set; }
    public double[] Coefficients { get; set; }

    public NewPolynomial()
    {
    }

    public NewPolynomial(int degree, double[] coefficients)
    {
        Degree = degree;
        Coefficients = coefficients;
    }

    public double Evaluate(double x)
    {
        double result = 0;
        for (int i = 0; i <= Degree; i++)
        {
            result += Coefficients[i] * Math.Pow(x, i);
        }
        return result;
    }

    public NewPolynomial Add(NewPolynomial other)
    {
        int maxDegree = Math.Max(Degree, other.Degree);
        double[] resultCoefficients = new double[maxDegree + 1];

        for (int i = 0; i <= maxDegree; i++)
        {
            double thisCoefficient = (i <= Degree) ? Coefficients[i] : 0;
            double otherCoefficient = (i <= other.Degree) ? other.Coefficients[i] : 0;
            resultCoefficients[i] = thisCoefficient + otherCoefficient;
        }

        return new NewPolynomial(maxDegree, resultCoefficients);
    }

    public NewPolynomial Subtract(NewPolynomial other)
    {
        int maxDegree = Math.Max(Degree, other.Degree);
        double[] resultCoefficients = new double[maxDegree + 1];

        for (int i = 0; i <= maxDegree; i++)
        {
            double thisCoefficient = (i <= Degree) ? Coefficients[i] : 0;
            double otherCoefficient = (i <= other.Degree) ? other.Coefficients[i] : 0;
            resultCoefficients[i] = thisCoefficient - otherCoefficient;
        }

        return new NewPolynomial(maxDegree, resultCoefficients);
    }

    public NewPolynomial Multiply(NewPolynomial other)
    {
        int resultDegree = Degree + other.Degree;
        double[] resultCoefficients = new double[resultDegree + 1];

        for (int i = 0; i <= Degree; i++)
        {
            for (int j = 0; j <= other.Degree; j++)
            {
                resultCoefficients[i + j] += Coefficients[i] * other.Coefficients[j];
            }
        }

        return new NewPolynomial(resultDegree, resultCoefficients);
    }

    public void Print()
    {
        Console.Write("Polynomial: ");
        for (int i = Degree; i >= 0; i--)
        {
            if (Coefficients[i] != 0)
            {
                if (i != Degree)
                {
                    if (Coefficients[i] > 0)
                        Console.Write(" + ");
                    else
                        Console.Write(" - ");
                }
                else if (Coefficients[i] < 0)
                {
                    Console.Write("-");
                }

                if (Math.Abs(Coefficients[i]) != 1 || i == 0)
                    Console.Write($"{Math.Abs(Coefficients[i])}");

                if (i > 1)
                    Console.Write($"x^{i}");
                else if (i == 1)
                    Console.Write($"x");
            }
        }
        Console.WriteLine();
    }

    public void SaveToJson(string fileName)
    {
        PolynomialJsonRepresentation polynomialJson = new PolynomialJsonRepresentation(Degree, Coefficients);
        string json = JsonConvert.SerializeObject(polynomialJson);
        File.WriteAllText(fileName, json);
        Console.WriteLine($"Polynomial saved to {fileName}.");
    }


    public static NewPolynomial LoadFromJson(string fileName)
    {
        try
        {
            string json = File.ReadAllText(fileName);
            PolynomialJsonRepresentation polynomialJson = JsonConvert.DeserializeObject<PolynomialJsonRepresentation>(json);
            return new NewPolynomial(polynomialJson.Degree, polynomialJson.Coefficients);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"File {fileName} not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        return null;
    }
}

class Program
{
    static void Main(string[] args)
    {
        NewPolynomial poly1 = new NewPolynomial(2, new double[] { 1, 2, 3 });
        NewPolynomial poly2 = new NewPolynomial(2, new double[] { 4, 5, 6 });

        Console.WriteLine("Value of poly1 at x = 2: " + poly1.Evaluate(2));
        Console.WriteLine("Value of poly2 at x = 2: " + poly2.Evaluate(2));

        NewPolynomial sum = poly1.Add(poly2);
        NewPolynomial difference = poly1.Subtract(poly2);
        NewPolynomial product = poly1.Multiply(poly2);

        Console.WriteLine("Sum:");
        sum.Print();

        Console.WriteLine("Difference:");
        difference.Print();

        Console.WriteLine("Product:");
        product.Print();
 
        sum.SaveToJson("sum.json");
        difference.SaveToJson("difference.json");
        product.SaveToJson("product.json");

        NewPolynomial loadedSum = NewPolynomial.LoadFromJson("sum.json");
        NewPolynomial loadedDifference = NewPolynomial.LoadFromJson("difference.json");
        NewPolynomial loadedProduct = NewPolynomial.LoadFromJson("product.json");

        Console.WriteLine("Loaded Sum:");           
        loadedSum.Print();

        Console.WriteLine("Loaded Difference:");
        loadedDifference.Print();

        Console.WriteLine("Loaded Product:");
        loadedProduct.Print();
    }
}
