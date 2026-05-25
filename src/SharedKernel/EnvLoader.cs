namespace SharedKernel;

public static class EnvLoader
{
    public static void Load()
    {
        // 1. Procura pelo arquivo .env subindo os diretórios a partir do diretório atual
        var currentDir = Directory.GetCurrentDirectory();
        string? envPath = null;

        while (currentDir != null)
        {
            var testPath = Path.Combine(currentDir, ".env");
            if (File.Exists(testPath))
            {
                envPath = testPath;
                break;
            }
            currentDir = Directory.GetParent(currentDir)?.FullName;
        }

        // Se encontrou o .env, carrega as variáveis de ambiente no processo
        if (envPath != null)
        {
            foreach (var line in File.ReadAllLines(envPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var val = parts[1].Trim().Trim('"', '\'');

                if (!string.IsNullOrEmpty(key))
                {
                    Environment.SetEnvironmentVariable(key, val);
                }
            }
        }

        // 2. Constrói e define a ConnectionStrings__DefaultConnection dinamicamente
        var isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
                       || Environment.GetEnvironmentVariable("IS_CONTAINER") == "true";

        var dbHost = isContainer
            ? Environment.GetEnvironmentVariable("DB_HOST_DOCKER") ?? "db"
            : Environment.GetEnvironmentVariable("DB_HOST_LOCAL") ?? "localhost";

        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";
        var dbDatabase = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "ticket_db";
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

        var connectionString = $"Host={dbHost};Port={dbPort};Database={dbDatabase};Username={dbUser};Password={dbPassword}";
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", connectionString);
    }
}
