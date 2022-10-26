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
using ClassLibrary1.Helpers;
using Autodesk.Revit.DB.Structure.StructuralSections;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Autodesk.Revit.DB.Visual;
using System.Globalization;




namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SelectElements : IExternalCommand
    {

        //Find parameter using the Parameter's definition type.


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)

        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;


            
            //Building a string that contains all the structural elements (Container)
            //StringBuilder structuralElements = new StringBuilder();
            //AllStructuralElements allStructuralelements = new AllStructuralElements();

            //var allElements = HelperFunctions.GetConnectorElements(doc);
            //var allApaces = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement));

            //structuralElements = Mapper.MapAllComponents(allElements);

            

            FilteredElementCollector collection = new FilteredElementCollector(doc);
            ElementCategoryFilter allWalls = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            List<Wall> listOfAllWalls = collection.WherePasses(allWalls).WhereElementIsNotElementType().Cast<Wall>().ToList();
            List<Wall> exteriorWalls = new List<Wall>();
            List<Wall> interiorWalls = new List<Wall>();

            FilteredElementCollector beam_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allBeams = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            List<FamilyInstance> listOfAllBeams = beam_collector.WherePasses(allBeams).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();
            
            FilteredElementCollector column_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allColumns = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            List<FamilyInstance> listOfAllColumns = column_collector.WherePasses(allColumns).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            FilteredElementCollector deck_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allDecks = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            List<Floor> listOfAllDecks = deck_collector.WherePasses(allDecks).WhereElementIsNotElementType().Cast<Floor>().ToList();

            FilteredElementCollector foundation_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allFoundation = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            List<Element> listOfAllFoundation = foundation_collector.WherePasses(allFoundation).WhereElementIsNotElementType().Cast<Element>().ToList();

            // Creates a Lists of all exterior and interior walls 
            foreach (Wall element in listOfAllWalls)
            {
                var test = element.WallType.Function;
                if (test == WallFunction.Exterior)
                {
                    exteriorWalls.Add(element);
                }
                else if (test == WallFunction.Interior)
                {
                    interiorWalls.Add(element);
                }
            }

            // Assigns the revit parameters to the Outerwall constructor
            foreach (Element element in exteriorWalls)
            {
                // Creates the TypeId
                WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                // Change from var to int
                int typeID = walltype.Id.IntegerValue;

                //CompoundStructureLayer compoundStructureLayer = 
                //string MaterialID = compoundStructureLayer.MaterialId

                //Hvordan får man Structural material??
                //FamilyInstance familyInstance = doc.GetElement(element.GetTypeId()) as FamilyInstance;
                //var materialID = familyInstance.StructuralMaterialType;              
                string materialID = "Concrete";
                //WallType.CompoundStructure.Layers
                //FamilyInstance familyInstance = doc.GetElement(element.GetTypeId()) as FamilyInstance;


                //WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                
                

                // Maps the area of the wall
                double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                double area = RoundToSignificantDigits.RoundDigits(area1, 3);

                // Maps the thickness of the wall
                WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                double thickness1 = ImperialToMetricConverter.ConvertFromFeetToMeters(wallType.Width);
                double thickness = RoundToSignificantDigits.RoundDigits(thickness1, 2);


                OuterWall outerWall = new OuterWall(typeID, materialID, area, thickness);

            }

            // Assigns the revit parameters to the Innerwall constructor
            foreach (Element element in interiorWalls)
            {
                // Creates the TypeId
                WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                // Change from var to int
                int typeID = walltype.Id.IntegerValue;

                //Hvordan får man Structural material??
                //FamilyInstance familyInstance = doc.GetElement(element.GetTypeId()) as FamilyInstance;
                //var materialID = familyInstance.StructuralMaterialType;              
                string materialID = "Concrete";

                // Maps the area of the wall
                double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                double area = RoundToSignificantDigits.RoundDigits(area1, 3);

                // Maps the thickness of the wall
                WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                double thickness1 = ImperialToMetricConverter.ConvertFromFeetToMeters(wallType.Width);
                double thickness = RoundToSignificantDigits.RoundDigits(thickness1, 2);

                InnerWall innerWall = new InnerWall(typeID, materialID, area, thickness);

            }

            foreach (FamilyInstance familyInstance in listOfAllBeams)
            {

                // Creates the TypeId
                var cast = (Element)familyInstance;
                int typeID = cast.Id.IntegerValue;

                

                //Maps the material of the beam
                string materialID = familyInstance.StructuralMaterialType.ToString();


                //Maps the length of the beam
                double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(familyInstance.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());
                double length = RoundToSignificantDigits.RoundDigits(length1, 3);


                ////Maps the crossSectionArea based on the volume and the length
                //double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(familyInstance.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                //double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);
                //double crossSectionAreaVol = volume / length;

                //Crosssection area in cm2
                string crossSectionAreaString = familyInstance.LookupParameter("A").AsValueString();
                string[] crossSectionAreaSplitted = crossSectionAreaString.Split(' ');
                //Crosssection area in m2
                var crossSectionArea = SquaredcmToSquaredm.Convert(Double.Parse(crossSectionAreaSplitted[0].Replace('.', '.'), CultureInfo.InvariantCulture));

                Beam beam = new Beam(typeID, materialID, length, crossSectionArea);


            }

            foreach (FamilyInstance familyInstance in listOfAllColumns)
            {
                // Creates the TypeId
                var cast = (Element)familyInstance;
                int typeID = cast.Id.IntegerValue;

                //Maps the material of the beam
                string materialID = familyInstance.StructuralMaterialType.ToString();


                //Maps the length of the column
                double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(familyInstance.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM).AsDouble());
                double length = RoundToSignificantDigits.RoundDigits(length1, 3);

                ////Maps the crossSectionArea based on volume and length
                //double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(familyInstance.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                //double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);
                //double crossSectionArea = volume / length;

                //Crosssection area in cm2
                string crossSectionAreaString = familyInstance.LookupParameter("A").AsValueString();
                string[] crossSectionAreaSplitted = crossSectionAreaString.Split(' ');
                //Crosssection area in m2
                var crossSectionArea = SquaredcmToSquaredm.Convert(Double.Parse(crossSectionAreaSplitted[0].Replace('.', '.'), CultureInfo.InvariantCulture));

                Column column = new Column(typeID, materialID, length, crossSectionArea);
            }

            // Assigns the revit parameters to the Outerwall constructor
            foreach (Element element in listOfAllDecks)
            {
                // Creates the TypeId
                
                // Change from var to int
                int typeID = element.Id.IntegerValue;

                //Hvordan får man Structural material??
                //FamilyInstance familyInstance = doc.GetElement(element.GetTypeId()) as FamilyInstance;
                //var materialID = familyInstance.StructuralMaterialType;              
                string materialID = "Concrete";
                
                // Maps the area of the deck
                double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                double area = RoundToSignificantDigits.RoundDigits(area1, 3);

                // Maps the thickness of the wall
                double thickness1 = ImperialToMetricConverter.ConvertFromFeetToMeters(element.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble());
                double thickness = RoundToSignificantDigits.RoundDigits(thickness1, 3);



                Deck deck = new Deck(typeID, materialID, area, thickness);

            }

            // Assigns the revit parameters to the Outerwall constructor
            foreach (Element element in listOfAllFoundation)
            {
                  
                var test = element.GetType().Name;
                if ((test == "WallFoundation") || (test == "Floor"))
                {
                    // Creates the TypeId
                    int typeID = element.Id.IntegerValue;

                    //Hvordan får man Structural material??          
                    string materialID = "Concrete";
                    //string material = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsString();

                    // Maps the width of the foundation
                    double width1 = ImperialToMetricConverter.ConvertFromFeetToMeters(element.get_Parameter(BuiltInParameter.CONTINUOUS_FOOTING_WIDTH).AsDouble());
                    double width = RoundToSignificantDigits.RoundDigits(width1, 4);

                    // Maps the length of the foundation
                    double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(element.get_Parameter(BuiltInParameter.CONTINUOUS_FOOTING_LENGTH).AsDouble());
                    double length = RoundToSignificantDigits.RoundDigits(length1, 4);

                    // Maps the height of the foundation
                    double height1 = ImperialToMetricConverter.ConvertFromFeetToMeters(doc.GetElement(element.GetTypeId()).LookupParameter("Foundation Thickness").AsDouble());
                    double height = RoundToSignificantDigits.RoundDigits(height1, 4);

                    Foundation foundation = new Foundation(typeID, materialID, length, width, height);
                }

                else if (test == "FamilyInstance")
                {
                    // Creates the TypeId
                    int typeID = element.Id.IntegerValue;

                    //Hvordan får man Structural material??          
                    string materialID = "Concrete";
                    //string material = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsString();

                    // Maps the width of the foundation
                    double width1 = ImperialToMetricConverter.ConvertFromFeetToMeters(element.get_Parameter(BuiltInParameter.CONTINUOUS_FOOTING_WIDTH).AsDouble());
                    double width = RoundToSignificantDigits.RoundDigits(width1, 4);

                    // Maps the length of the foundation
                    double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(element.get_Parameter(BuiltInParameter.CONTINUOUS_FOOTING_LENGTH).AsDouble());
                    double length = RoundToSignificantDigits.RoundDigits(length1, 4);

                    // Maps the height of the foundation
                    //This one is not working
                    double height1 = ImperialToMetricConverter.ConvertFromFeetToMeters(doc.GetElement(element.GetTypeId()).LookupParameter("Thickness").AsDouble());
                    double height = RoundToSignificantDigits.RoundDigits(height1, 4);

                    Foundation foundation = new Foundation(typeID, materialID, length, width, height);
                }
                

                
            }

            return Result.Succeeded;

            


        }


    }
}


    


