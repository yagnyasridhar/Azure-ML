
public class Rootobject
{
    public Results Results { get; set; }
}

public class Results
{
    public Output1 output1 { get; set; }
}

public class Output1
{
    public string type { get; set; }
    public Value value { get; set; }
}

public class Value
{
    public string[] ColumnNames { get; set; }
    public string[] ColumnTypes { get; set; }
    public string[][] Values { get; set; }
}

public class LogicAppInput
{
    public string User { get; set; }
    public string Item1 { get; set; }
    public string Item2 { get; set; }
    public string Item3 { get; set; }
    public string Item4 { get; set; }
    public string Item5 { get; set; }
}