using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandLine;

public class Program
{
    public static readonly string adminSectionsPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "admin",
        "Views",
        "Shared",
        "Components",
        "Section"
    );
    public static readonly string adminSectionRegisterPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "admin",
        "Views",
        "Shared",
        "Components",
        "PageSections",
        "Default.cshtml"
    );
    public static readonly string clientSectionsPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "www",
        "Views",
        "Shared",
        "Components"
    );

    public class Options
    {
        [Value(0, MetaName = "name", Required = true, HelpText = "The section name to create")]
        public string Name { get; set; }

        [Option('r', "pr", Default = 0, HelpText = "Number of paragraphs to include in the template.")]
        public int Paragraphs { get; set; }

        [Option('i', "im", Default = 0, HelpText = "Number of images to include in the template.")]
        public int Images { get; set; }

        [Option('t', "txt", Default = 0, HelpText = "Number of textareas to include in the template.")]
        public int Textareas { get; set; }
    }

    public static void Main(string[] args)
    {
        Parser
            .Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts => ExecuteCommand(opts.Name, opts.Paragraphs, opts.Images, opts.Textareas))
            .WithNotParsed<Options>((errs) => HandleParseError(errs));
    }

    private static void ExecuteCommand(string name, int paragraphs, int images, int textareas)
    {
        string sectionNameWithExtension = $@"_{name}Section.cshtml";
        string sectionName = $@"_{name}Section";
        GenerateAdminTemplates(name, sectionNameWithExtension, sectionName, paragraphs, images, textareas);
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

    private static void GenerateAdminTemplates(
        string name,
        string sectionNameWithExtension,
        string sectionName,
        int paragraphs,
        int images,
        int textareass
    )
    {
        try
        {
            string adminSectionRegisterContent = ReadEmbeddedResource("CLI.adminRegisterContent.txt");
            adminSectionRegisterContent = adminSectionRegisterContent.Replace("{name}", name);
            adminSectionRegisterContent = adminSectionRegisterContent.Replace("{sectionName}", sectionName);

            var test = "berat".ToString();

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

            if (paragraphs > 0)
            {
                string adminParagraphTemplate = ReadEmbeddedResource("CLI.adminParagraph.txt");
                for (int i = 1; i < paragraphs + 1; i++)
                {
                    string newAdminParagraphTemplate = adminParagraphTemplate.Replace("paramText1", "paramText" + i);
                    newAdminParagraphTemplate = newAdminParagraphTemplate.Replace("Title 1", "Title " + i);
                    adminSectionContent += newAdminParagraphTemplate;
                }
            }

            if (images > 0)
            {
                string adminImageTemplate = ReadEmbeddedResource("CLI.adminImage.txt");
                for (int i = 1; i < images + 1; i++)
                {
                    string newAdminImageTemplate = adminImageTemplate.Replace("paramMediaId1", "paramMediaId" + i);
                    newAdminImageTemplate = newAdminImageTemplate.Replace("FileId = 1", "FileId = " + i);
                    adminSectionContent += newAdminImageTemplate;
                }
            }
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

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        Console.WriteLine("Error parsing arguments.", errs);
    }
}
