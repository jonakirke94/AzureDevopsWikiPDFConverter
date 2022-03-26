using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsWikiPDFConverter
{
    internal static class SELECTORS
    {
        public static By TOP_LEVEL_ITEMS = By.CssSelector("tr[aria-level='2']");
        public static By ITEM_TITLE = By.CssSelector(".bolt-list-cell-child.bolt-list-cell-text.tree-name-cell");
        public static By ITEM_EXPANDABLE_ARROW = By.CssSelector(".bolt-tree-expand-button");

        public static By PRINT_MENU = By.Id("__bolt-header-command-bar-menu-button1");
        public static By PRINT_BUTTON = By.Id("__bolt-print-page-text");

        public static By SUB_LEVEL_ITEMS(int level)
        {
            return By.CssSelector($"tr[aria-level='{level}']");
        }            
    }
}
