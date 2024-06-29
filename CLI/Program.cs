using CommandLine;
using System;
using System.Diagnostics;
using System.IO;

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
        FormatProject(Path.Combine(Directory.GetCurrentDirectory(), "admin"));
        FormatProject(Path.Combine(Directory.GetCurrentDirectory(), "www"));
    }

    private static void GenerateClientTemplates(string name, string sectionNameWithExtension)
    {
        try
        {
            string clientSectionContent = $@"
            @model SectionViewModel
            @inject IUnitOfWork _uow
            @{{
            }}
            <div class=""container"">
            <div class=""row justify-center align-items-center"">
                <div class=""col-12"">
                    <h1>{name}</h1>
                </div>
            </div>
            ";
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
            string adminSectionRegisterContent = $@"
            <div class=""row"">
                <div class=""col-sm-12 col-md-12"">
                    <a data-url='@Url.Action(""sectionManager"", ""page"", new {{ sectionName = ""{sectionName}"" }})'
                    class=""section_add__btn btn btn-block btn-outline-info"">
                        {name} Section
                    </a>
                </div>
            </div>
            <br />
            ";
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
            string adminSectionContent = $@"
            @model SectionViewModel
            @inject IUnitOfWork _uow
            @{{
                Language language;
                if (_uow.Cookie.GetUserLanguageId != 0)
                {{
                    language = _uow.Language.GetById(_uow.Cookie.GetUserLanguageId);
                }}
                else
                {{
                    language = _uow.Language.GetIsRootLang();
                }}
            }}
            <input type=""hidden"" name=""sectionName"" value=""{sectionName}"" />
            <input type=""hidden"" name=""order"" value=""@Model.order"" />
            <input type =""hidden"" name = ""langId"" value =""@language.Id"" />
            <div class=""form-group"">
                <label> Title </label>
                <input type=""text"" class=""form-control"" name=""title"" value=""@Model.title"" onkeypress=""changeTitle(this);"" onblur=""changeTitle(this);"" />
            </div>
            ";
            File.WriteAllText(Path.Combine(adminSectionsPath, sectionNameWithExtension), adminSectionContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private static void FormatProject(string projectPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "format",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = projectPath
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            Console.WriteLine(output);
        }
    }

}
