namespace BxNiom.Math;

public class FilteredDouble {
    private readonly List<double> _values = new();

    public FilteredDouble(FilterAlgorithm algorithm, int maxPoints = 5) {
        MaxPoints = maxPoints;
        Algorithm = algorithm;
    }

    public int             MaxPoints   { get; set; }
    public FilterAlgorithm Algorithm   { get; set; }
    public double          LastValue   { get; private set; } = double.NaN;
    public double          LastAverage { get; private set; } = double.NaN;

    public double Add(double value) {
        var total = value;
        _values.ForEach(v => total += v);
        var newAverage = total / (_values.Count + 1);

        var newValue = Algorithm switch {
            FilterAlgorithm.DecayAverage => (newAverage + (double.IsNaN(LastAverage) ? newAverage : LastAverage)) /
                                            2,
            FilterAlgorithm.WindowedAverage or _ => (newAverage + (double.IsNaN(LastValue) ? value : LastValue)) / 2
        };

        LastAverage = newAverage;
        LastValue   = newValue;

        _values.Add(value);
        while (_values.Count > MaxPoints) {
            _values.RemoveAt(0);
        }

        return newValue;
    }

    public void Reset() {
        LastAverage = double.NaN;
        LastValue   = double.NaN;
        _values.Clear();
    }
}