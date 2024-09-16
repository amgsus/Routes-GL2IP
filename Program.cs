using System.Net;

const string COUNTRY = "630336"; // BY

const string DATABASE_PATH = "GeoLite2-Country-Blocks-IPv4-240913.csv";

const string ROUTE_SCRIPT_PATH = "routes.txt";

const string GATEWAY = "0.0.0.0";

string[] database = File.ReadAllLines(DATABASE_PATH);

List<string> subnets = new List<string>();

foreach (string line in database)
{
    string[] tokens = line.Split(',');

    if (tokens[1] != COUNTRY) 
        continue;

    subnets.Add(tokens[0]);
}

Console.WriteLine($"Found {subnets.Count} subnets");

if (subnets.Count == 0)
    return; // Exit

List<string> routes = new(subnets.Count);

foreach (string sn in subnets)
{
    string[] tokens = sn.Split("/");
    string subnet = tokens[0];
    string mask = CidrToMask(int.Parse(tokens[1]));

    string route = $"route ADD {subnet} MASK {mask} {GATEWAY}"; // Example: route ADD 157.0.0.0 MASK 255.0.0.0 157.55.80.1
    routes.Add(route);

    Console.WriteLine(route);
}

File.WriteAllLines(ROUTE_SCRIPT_PATH, routes);
static string CidrToMask(int cidr)
{
    var mask = (cidr == 0) ? 0 : uint.MaxValue << (32 - cidr);
    var bytes = BitConverter.GetBytes(mask).Reverse().ToArray();
    return new IPAddress(bytes).ToString();
}
