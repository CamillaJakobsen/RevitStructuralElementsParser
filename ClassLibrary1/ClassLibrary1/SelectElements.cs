using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Forms;
using Autodesk.Revit.Creation;
using System.Xml.Linq;
using Document = Autodesk.Revit.DB.Document;
using Application = Autodesk.Revit.ApplicationServices.Application;
using ClassLibrary1.Models;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    //[Regeneration(RegenerationOption.Manual)]
    public class SelectElements : IExternalCommand
    {
        //Find parameter using the Parameter's definition type.
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)

        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //StructuralElements structuralelements = new StructuralElements();

            // var allElements = HelperFunctions.GetConnectorElements(doc);
            // var allApaces = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)); 

            //structuralElements = Mapper.MapAllComponents(allElements);

            FilteredElementCollector collection = new FilteredElementCollector(doc); 

           
            ElementCategoryFilter allStructuralElements = new ElementCategoryFilter(BuiltInCategory.OST_Walls);


            foreach (var element in allStructuralElements)
            {
                
                var test = element.Category.Name;

                
                    // Den bliver casted
                    var carsten = (AnalyticalModelSurface)element;
                    var bIM7AATypeID = 123456;
                    var materialID = carsten.GetTypeId();
                    double area1 = carsten.GetMaterialArea(materialID, false);
                    //double thickness =carsten.
        
                    
                    OuterWall outerWall = new OuterWall(bIM7AATypeID, materialID, area1, thickness);
                




            }



            //Reference reference = uidoc.Selection.PickObject(ObjectType.Element);
            //Element element = uidoc.Document.GetElement(reference);
            //using (Transaction tx = new Transaction(doc))
            //{
            //    tx.Start("transaction");
            //    TaskDialog.Show("title :) ", element.Name);
            //    tx.Commit();
            //}


            return Result.Succeeded;


        }



    }
}


//public class SharedParameterElement : ParameterElement