//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Selection;
//using Autodesk.Revit.DB.Architecture;
//using System.Windows.Forms;

//namespace ClassLibrary1
//{

//    [Transaction(TransactionMode.Manual)]
//    //[Regeneration(RegenerationOption.Manual)]
//    public class FetchElements : IExternalCommand
//    {
//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//        {
//            //Get application and document objects
//            UIApplication uiapp = commandData.Application;
//            Document doc = uiapp.ActiveUIDocument.Document;
//            //Document doc = commandData.Application.ActiveUIDocument.Document;

//            //var filterElement = new FilteredElementCollector(doc).WhereElementIsNotElementType().WhereElementIsViewIndependent().Where(x => x.Category !=
//            //null && x.get_BoundingBox(null) != null && x.get_Geometry(new Options() { ComputeReferences = true }) != null);




//            //StringBuilder sb = new StringBuilder();

//            //foreach (Element e in filterElement)
//            //{
//            //    if (e.IsValidObject)
//            //    {
//            //        sb.AppendLine("Element Category : " + e.Category.Name + ", Name : " + e.Name + ", Element Id : " + e.Id + Environment.NewLine);
//            //    }
//            //}

//            //MessageBox.Show(sb.ToString());

            

//            //return Result.Succeeded;
           

//        }
//    }
//}


////ElementCategory = Element.category.Name