using System;

// Main program class
class Program
{
    static void Main(string[] args)
    {
        // Create and start the shipping quote application
        var app = new ShippingQuoteApplication();
        app.Start();
    }
}

// Class representing shipping quote application events
class ShippingQuoteEventArgs : EventArgs
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public double? Value { get; set; }
}

// Class handling user input and validation
class InputHandler
{
    // Constants for validation
    private const int MaxWeight = 50;
    private const int MaxTotalDimensions = 50;

    // Events for input validation results
    public event EventHandler<ShippingQuoteEventArgs> WeightValidated;
    public event EventHandler<ShippingQuoteEventArgs> DimensionsValidated;

    // Method to process weight input
    public void ProcessWeight(double weight)
    {
        var args = new ShippingQuoteEventArgs
        {
            IsValid = weight <= MaxWeight,
            Value = weight,
            Message = weight > MaxWeight ? 
                "Package too heavy to be shipped via Package Express. Have a good day." : 
                "Weight validated successfully."
        };
        
        WeightValidated?.Invoke(this, args);
    }

    // Method to process dimensions input
    public void ProcessDimensions(double width, double height, double length)
    {
        double totalDimensions = width + height + length;
        var args = new ShippingQuoteEventArgs
        {
            IsValid = totalDimensions <= MaxTotalDimensions,
            Value = totalDimensions,
            Message = totalDimensions > MaxTotalDimensions ?
                "Package too big to be shipped via Package Express." :
                "Dimensions validated successfully."
        };

        DimensionsValidated?.Invoke(this, args);
    }
}

// Class handling shipping calculations
class ShippingCalculator
{
    // Calculate shipping cost
    public double CalculateQuote(double weight, double width, double height, double length)
    {
        return (width * height * length * weight) / 100;
    }
}

// Main application class coordinating all operations
class ShippingQuoteApplication
{
    private readonly InputHandler _inputHandler;
    private readonly ShippingCalculator _calculator;
    private double _weight;
    private double _width;
    private double _height;
    private double _length;

    public ShippingQuoteApplication()
    {
        _inputHandler = new InputHandler();
        _calculator = new ShippingCalculator();

        // Subscribe to input handler events
        _inputHandler.WeightValidated += OnWeightValidated;
        _inputHandler.DimensionsValidated += OnDimensionsValidated;
    }

    // Start the application
    public void Start()
    {
        Console.WriteLine("Welcome to Package Express. Please follow the instructions below.");
        ProcessWeightInput();
    }

    // Handle weight input
    private void ProcessWeightInput()
    {
        Console.WriteLine("Please enter the package weight:");
        _weight = Convert.ToDouble(Console.ReadLine());
        _inputHandler.ProcessWeight(_weight);
    }

    // Handle dimensions input
    private void ProcessDimensionsInput()
    {
        Console.WriteLine("Please enter the package width:");
        _width = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Please enter the package height:");
        _height = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Please enter the package length:");
        _length = Convert.ToDouble(Console.ReadLine());

        _inputHandler.ProcessDimensions(_width, _height, _length);
    }

    // Event handler for weight validation
    private void OnWeightValidated(object sender, ShippingQuoteEventArgs e)
    {
        Console.WriteLine(e.Message);
        if (e.IsValid)
        {
            ProcessDimensionsInput();
        }
    }

    // Event handler for dimensions validation
    private void OnDimensionsValidated(object sender, ShippingQuoteEventArgs e)
    {
        Console.WriteLine(e.Message);
        if (e.IsValid)
        {
            double quote = _calculator.CalculateQuote(_weight, _width, _height, _length);
            Console.WriteLine($"Your estimated total for shipping this package is: ${quote:F2}");
            Console.WriteLine("Thank you!");
        }
    }
}