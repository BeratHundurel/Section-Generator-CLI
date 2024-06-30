using CommandLine;
using System;
using System.IO;
using System.Reflection;

public class Program
{
    public static readonly string adminSectionsPath = Path.Combine(Directory.GetCurrentDirectory(), "admin", "Views", "Shared", "Components", "Section");
    public static readonly string adminSectionRegisterPath = Path.Combine(Directory.GetCurrentDirectory(), "admin", "Views", "Shared", "Components", "PageSections", "Default.cshtml");
    public static readonly string clientSectionsPath = Path.Combine(Directory.GetCurrentDirectory(), "www", "Views", "Shared", "Components");

    public class Options
    {
        [Value(0, MetaName = "name", Required = true, HelpText = "The section name to create")]
        public string Name { get; set; }
    }

    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
               .WithParsed<Options>(opts => ExecuteCommand(opts.Name));
    }

    private static void ExecuteCommand(string name)
    {
        string sectionNameWithExtension = $@"_{name}Section.cshtml";
        string sectionName = $@"_{name}Section";
        GenerateAdminTemplates(name, sectionNameWithExtension, sectionName);
        GenerateClientTemplates(name, sectionNameWithExtension);
    }

    private static void GenerateClientTemplates(string name, string sectionNameWithExtension)
    {
        try
        {
            string clientSectionContent = ReadEmbeddedResource("CLI.clientSectionContent.txt");
            clientSectionContent = clientSectionContent.Replace("{name}", name);
            File.WriteAllText(Path.Combine(clientSectionsPath, sectionNameWithExtension), clientSectionContent);
            Console.WriteLine("Client section created successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void GenerateAdminTemplates(string name, string sectionNameWithExtension, string sectionName)
    {
        try
        {
            string adminSectionRegisterContent = ReadEmbeddedResource("CLI.adminRegisterContent.txt");
            adminSectionRegisterContent = adminSectionRegisterContent.Replace("{name}", name);
            adminSectionRegisterContent = adminSectionRegisterContent.Replace("{sectionName}", sectionName);

            string registerContent = File.ReadAllText(adminSectionRegisterPath);
            int registerInsertIndex = registerContent.IndexOf("}");
            registerContent = registerContent.Insert(registerInsertIndex + 1, adminSectionRegisterContent);

            File.WriteAllText(adminSectionRegisterPath, registerContent);
            Console.WriteLine("Admin section registered successfully");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        try
        {
            string adminSectionContent = ReadEmbeddedResource("CLI.adminSectionContent.txt");
            adminSectionContent = adminSectionContent.Replace("{sectionName}", sectionName);
            File.WriteAllText(Path.Combine(adminSectionsPath, sectionNameWithExtension), adminSectionContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static string ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}
