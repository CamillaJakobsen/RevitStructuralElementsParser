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
    [Regeneration(RegenerationOption.Manual)]
    public class SelectElements : IExternalCommand
    {
        private const string V = "Exterior";

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

            ElementCategoryFilter allWalls = new ElementCategoryFilter(BuiltInCategory.OST_Walls);

            List<Wall> listOfAllWalls = collection.WherePasses(allWalls).WhereElementIsNotElementType().Cast<Wall>().ToList();

            //ElementClassFilter elementClass = ElementClassFilter(WallType(WallFunction.Exterior));


            //WallType wallTypeExterior = new WallType(WallFunction.Exterior);

            //List<Wall> listWallTypeExterior = collection.WherePasses(wallTypeExterior).WhereElementIsNotElementType().Cast<Wall>().ToList();

            //ElementCategoryFilter outerWall = wallTypeExterior;

            List<Wall> exteriorWalls = new List<Wall>();




            foreach (Wall element in listOfAllWalls)
            {

                //string wallName = element.Category.Name;

                //var test = element.Category.Name;
                //if (ElementCategoryFilter() = BuiltInCategory.OST_Walls)

                var test = element.WallType.Function;
                if (test == WallFunction.Exterior)
                {
                    exteriorWalls.Add(element);

                }

                

            }

            foreach (Element element in exteriorWalls)
            {
                //var bIM7AATypeID = 123456;
                WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                var bIM7AATypeID = walltype.Id;
                //var materialID = "Concrete";
                //double areaInFeet = element.get_Parameter(BuiltInParameter

                //var Carsten = (AnalyticalModelSurface)element;


                double areaInFeet = element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();


                //Carsten.GetMaterialArea

                // Remember this is not in metrics.
                WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                double thicknessInFeet = wallType.Width;
                

            }



            //}





            //OuterWall outerWall = new OuterWall(bIM7AATypeID, materialID, area1, thickness);


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