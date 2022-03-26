using AzureDevopsWikiPDFConverter;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WindowsInput;
using WindowsInput.Native;

string OUTPUT_DIR = "C:\\downloads\\";
string AZURE_DEVOPS_WIKI_URL = "https://dev.azure.com/{TENANT}/{PROJECT}/_wiki/wikis/{PROJECT_NAME}/{WIKI_ID}/{WIKI_NAME}";
string CHROME_USER_PROFILE = "\\Users\\Jonathan\\AppData\\Local\\Google\\Chrome\\User Data";

InputSimulator sim = new();

try
    {
    ChromeOptions chromeOptions = new();
    chromeOptions.AddArgument("--verbose");
    chromeOptions.AddArgument("--whitelisted-ips=''");
    chromeOptions.AddArgument($"--user-data-dir=C:{CHROME_USER_PROFILE}");

    var driver = new ChromeDriver(chromeOptions);

    EnsureOutputDirExists(OUTPUT_DIR);

    driver.Navigate().GoToUrl(AZURE_DEVOPS_WIKI_URL);

    SaveFrontpage(driver, OUTPUT_DIR);

    var topLevelMenuList = driver.FindElements(SELECTORS.TOP_LEVEL_ITEMS);
    foreach (var toplevelMenu in topLevelMenuList)
    {
        SaveItemsRecursively(driver, toplevelMenu, path: OUTPUT_DIR, level: 2);
    }

    driver.Quit();

}
catch (Exception e) {
    Console.WriteLine("\nException Caught!");
    Console.WriteLine("Message :{0} ", e.Message);
}

void SaveItemsRecursively(ChromeDriver driver, IWebElement element, string path, int level)
{
    // Navigate to element
    element.Click();

    // We need to look ahead to see if there are any children.
    // If there are children we will create a directory
    var nextLevel = level + 1;
    var subLevetItems = GetSubLevelItems(driver, nextLevel);

    var currentTitle = element.FindElement(SELECTORS.ITEM_TITLE).Text;

    var absoluteFilePath = Path.Combine(path, currentTitle);

    var absoluteFilename = absoluteFilePath;    

    if (subLevetItems.Any())
    {
        // If there are sub items then the item should be put inside the same folder
        var subDirectory = Path.Combine(path, currentTitle);

        if (!Directory.Exists(absoluteFilePath))
        {
            Directory.CreateDirectory(absoluteFilePath);
        }

        absoluteFilename = Path.Combine(absoluteFilePath, currentTitle);
    }

    OpenPrintDialog(driver);

    // Let page render before we attempt to print
    Thread.Sleep(1000);
    HandlePrintDialog(absoluteFilename);

    foreach (var sublevelMenu in subLevetItems)
    {
        SaveItemsRecursively(driver, sublevelMenu, path: absoluteFilePath, level: nextLevel);
    }

    // Close the current menu
    if (subLevetItems.Any())
    {
        element.FindElement(SELECTORS.ITEM_EXPANDABLE_ARROW).Click();
    }
}

IEnumerable<IWebElement> GetSubLevelItems(ChromeDriver driver, int level)
{
    try
    {
        return driver.FindElements(SELECTORS.SUB_LEVEL_ITEMS(level));
    }
    catch (Exception)
    {
        return new List<IWebElement>();
    
    }
}

void OpenPrintDialog(ChromeDriver driver)
{
    Thread.Sleep(1500);

    // Open menu
    driver.FindElement(SELECTORS.PRINT_MENU).Click();

    // Click print
    driver.FindElement(SELECTORS.PRINT_BUTTON).Click();
}

void HandlePrintDialog(string absolutePath)
{
    // Press save in print dialog
    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

    Thread.Sleep(1500);
    // Reset absolute path in directory dialog
    sim.Keyboard.KeyPress(VirtualKeyCode.DELETE);

    // Enter absolute path in directory dialog
    sim.Keyboard.TextEntry(absolutePath);

    // Press save in directory dialog
    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
}

void EnsureOutputDirExists(string outputDir) {  
    
    var outputDirExists = Directory.Exists(outputDir);

    if (!outputDirExists)
    {
        Directory.CreateDirectory(outputDir);
    }
}

void SaveFrontpage(ChromeDriver driver, string path)
{
    OpenPrintDialog(driver);

    // Let page render before we attempt to print
    Thread.Sleep(1000);

    var wikiFrontPagePath = Path.Combine(path, "Wiki");
    HandlePrintDialog(wikiFrontPagePath);
}