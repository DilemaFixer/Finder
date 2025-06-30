namespace CsFinder;

public class Query
{
    private string _query;
    private string[] _inputs;
    public string[] Inputs => (string[])_inputs.Clone();
    public string? Outputs { get; private set; }
    private bool _haveRow;

    public Query(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        _query = query;
    }

    public void Parse()
    {
        string[] _temp = [];

        if (_query.Contains("->"))
        {
            _haveRow = true;
            _temp = _query.Split("->");
            _inputs = _temp[0].Trim().Split(",");
            Outputs = _temp[1];
        }
        else
            _inputs = _query.Trim().Split(",");
    }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Outputs) && _haveRow)
            return false;

        return true;
    }
}