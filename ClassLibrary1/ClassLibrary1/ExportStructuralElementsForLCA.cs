﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.DB.Document;
using StructuralElementsExporter.Models;
using StructuralElementsExporter.Helpers;
using System.IO;
using Newtonsoft.Json;
using StructuralElementsExporter.Models.Containers;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using System.Security.Policy;
using File = System.IO.File;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB.Structure;

namespace StructuralElementsExporter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExportStructuralElementsForLCA : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)

        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector wall_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allWalls = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            List<Wall> listOfAllWalls = wall_collector.WherePasses(allWalls).WhereElementIsNotElementType().Cast<Wall>().ToList();
            List<Wall> listOfAllExteriorWalls = new List<Wall>();
            List<Wall> listOfAllInteriorWalls = new List<Wall>();

            FilteredElementCollector beam_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allBeams = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            List<FamilyInstance> listOfAllBeams = beam_collector.WherePasses(allBeams).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            FilteredElementCollector column_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allColumns = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            List<FamilyInstance> listOfAllColumns = column_collector.WherePasses(allColumns).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            FilteredElementCollector deck_collector = new FilteredElementCollector(doc);
            FilteredElementCollector roof_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allDecks = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            ElementCategoryFilter allRoofs = new ElementCategoryFilter(BuiltInCategory.OST_Roofs);
            List<Element> listOfAllDecks = deck_collector.WherePasses(allDecks).WhereElementIsNotElementType().Cast<Element>().ToList();
            List<Element> listOfAllRoofs = roof_collector.WherePasses(allRoofs).WhereElementIsNotElementType().Cast<Element>().ToList();
            listOfAllDecks.AddRange(listOfAllRoofs);
                
            FilteredElementCollector foundation_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allFoundation = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            List<Element> listOfAllFoundation = foundation_collector.WherePasses(allFoundation).WhereElementIsNotElementType().Cast<Element>().ToList();

            FilteredElementCollector rebar_collector = new FilteredElementCollector(doc);
            FilteredElementCollector areaReinforcement_collector = new FilteredElementCollector(doc);
            ElementCategoryFilter allRebar = new ElementCategoryFilter(BuiltInCategory.OST_Rebar);
            List<Element> listOfAllReinforcement = rebar_collector.WherePasses(allRebar).WhereElementIsNotElementType().Cast<Element>().ToList();
            ElementCategoryFilter allAreaReinforcement = new ElementCategoryFilter(BuiltInCategory.OST_AreaRein);
            List<Element> listOfAllAreaReinforcement = areaReinforcement_collector.WherePasses(allAreaReinforcement).WhereElementIsNotElementType().Cast<Element>().ToList();
            listOfAllReinforcement.AddRange(listOfAllAreaReinforcement);

            // Creates a Lists of all exterior and interior walls 
            foreach (Wall element in listOfAllWalls)
            {
                var test = element.WallType.Function;
                if (test == WallFunction.Exterior)
                {
                    listOfAllExteriorWalls.Add(element);
                }
                else if (test == WallFunction.Interior)
                {
                    listOfAllInteriorWalls.Add(element);
                }

            }

            ExteriorWalls exteriorWalls = new ExteriorWalls();
            // Assigns the revit parameters to the Outerwall constructor
            foreach (Element element in listOfAllExteriorWalls)
            {
                var structuralUsage = element.LookupParameter("Structural Usage").AsValueString();

                if (structuralUsage == "Bearing")
                {
                    // Creates the TypeId
                    WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                    // Change from var to int
                    int typeID = element.Id.IntegerValue;

                    //Creates Structural material
                    string quality = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();

                    // Maps the area of the wall
                    double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                    double area = Math.Round(RoundToSignificantDigits.RoundDigits(area1, 4), 5);

                    //if material consists of more than one layer
                    CompoundStructure wallLayers = walltype.GetCompoundStructure();

                    List<CompoundStructureLayer> layers = new List<CompoundStructureLayer>(wallLayers.GetLayers());

                    string material;
                    double thickness;
                    double weight;

                    foreach (CompoundStructureLayer layer in layers)
                    {
                        string testLayer = layer.Function.ToString();

                        if (testLayer == "Structure")
                        {
                            Material structuralLayerDeck = doc.GetElement(layer.MaterialId) as Material;

                            material = structuralLayerDeck.Name;
                            thickness = RoundToSignificantDigits.RoundDigits(ImperialToMetricConverter.ConvertFromFeetToMeters(layer.Width), 3);

                            weight = 0;

                            ExteriorWall exteriorWall = new ExteriorWall(typeID, material, quality, area, thickness, weight);
                            exteriorWalls.AddExteriorWall(exteriorWall);

                        }

                    }
                }
            }

            InteriorWalls interiorWalls = new InteriorWalls();
            // Assigns the revit parameters to the Innerwall constructor
            foreach (Element element in listOfAllInteriorWalls)
            {
                var structuralUsage = element.LookupParameter("Structural Usage").AsValueString();

                if (structuralUsage == "Bearing")
                {

                    // Creates the TypeId
                    WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                    // Change from var to int
                    int typeID = element.Id.IntegerValue;

                    //Creates Structural material
                    string quality = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();
                    

                    // Maps the area of the wall
                    double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                    double area = RoundToSignificantDigits.RoundDigits(area1, 4);

                    //if material consists of more than one layer
                    CompoundStructure wallLayers = walltype.GetCompoundStructure();

                    List<CompoundStructureLayer> layers = new List<CompoundStructureLayer>(wallLayers.GetLayers());

                    string material;
                    double thickness;

                    foreach (CompoundStructureLayer layer in layers)
                    {
                        string testLayer = layer.Function.ToString();

                        double weight = 0;

                        if (testLayer == "Structure")
                        {
                            Material structuralLayerWall = doc.GetElement(layer.MaterialId) as Material;

                            material = structuralLayerWall.Name;
                            thickness = RoundToSignificantDigits.RoundDigits(ImperialToMetricConverter.ConvertFromFeetToMeters(layer.Width), 3);

                            InteriorWall interiorWall = new InteriorWall(typeID, material, quality, area, thickness, weight);
                            interiorWalls.AddInteriorWall(interiorWall);

                        }

                    }

                }
            }

            Beams beams = new Beams();
            // Assigns the revit parameters to the Beam constructor
            foreach (FamilyInstance familyInstance in listOfAllBeams)
            {

                // Creates the TypeId
                var cast = (Element)familyInstance;
                FamilySymbol familySymbol = doc.GetElement(familyInstance.GetTypeId()) as FamilySymbol;
                Family family = doc.GetElement(familyInstance.GetTypeId()) as Family;

                int typeID = cast.Id.IntegerValue;

                //Maps the material of the beam
                string material = familyInstance.StructuralMaterialType.ToString();

                string quality = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsValueString();
                    if (quality == "<By Category>")
                    {
                        quality = doc.GetElement(cast.GetTypeId()).LookupParameter("Structural Material").AsValueString();
                    }
                   

                ////Maps the crossSectionArea based on the volume and the length
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(familyInstance.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                //Alternative way to get volume
                //var volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(cast.LookupParameter("Volume").AsDouble());
                double volume = Math.Round(RoundToSignificantDigits.RoundDigits(volume1, 4), 5);

                double weight = 0;
                if (material.Contains("Steel"))
                {
                    weight = Math.Round(RoundToSignificantDigits.RoundDigits(WeightOfSteel.Convert(volume), 4), 5);
                }
                else if (quality.Contains("Aluminum"))
                {
                    weight = Math.Round(RoundToSignificantDigits.RoundDigits(WeightOfAluminium.Convert(volume), 4), 5);
                }

                Beam beam = new Beam(typeID, material, quality, volume, weight);
                beams.AddBeam(beam);

            }

            Columns columns = new Columns();
            // Assigns the revit parameters to the Column constructor
            foreach (FamilyInstance familyInstance in listOfAllColumns)
            {
                // Creates the TypeId
                var cast = (Element)familyInstance;
                int typeID = cast.Id.IntegerValue;

                //Maps the material of the column
                string material = familyInstance.StructuralMaterialType.ToString();

                string quality = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsValueString();
                if (quality == "<By Category>")
                {
                    quality = doc.GetElement(cast.GetTypeId()).LookupParameter("Structural Material").AsValueString();
                }
                


                //Maps the length of the column
                double length1 = ImperialToMetricConverter.ConvertFromFeetToMeters(familyInstance.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM).AsDouble());
                double length = RoundToSignificantDigits.RoundDigits(length1, 3);

                ////Maps the crossSectionArea based on volume and length
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(familyInstance.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                double volume = Math.Round(RoundToSignificantDigits.RoundDigits(volume1, 4), 5);

                double weight = 0;
                if (material.Contains("Steel"))
                {
                    weight = Math.Round(RoundToSignificantDigits.RoundDigits(WeightOfSteel.Convert(volume), 4), 5);
                }

                Column column = new Column(typeID, material, quality, volume, weight);
                columns.AddColumn(column);

            }

            Decks decks = new Decks();
            // Assigns the revit parameters to the Deck constructor
            foreach (Element element in listOfAllDecks)
            {
                Parameter isStructural = element.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL);
                if (isStructural!=null)
                {
                    if (element is Floor)
                    {
                        // Creates the TypeId
                        int typeID = element.Id.IntegerValue;


                        //Hvordan får man Structural material??
                        string quality = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();

                        // Maps the area of the deck
                        double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                        double area = RoundToSignificantDigits.RoundDigits(area1, 4);
                        //string material = carsten.FloorType.LookupParameter("Structural Material").AsValueString();

                        FloorType carsten = doc.GetElement(element.GetTypeId()) as FloorType;

                        //if material consists of more than two layers
                        CompoundStructure deckLayers = carsten.GetCompoundStructure();

                        List<CompoundStructureLayer> layers = new List<CompoundStructureLayer>(deckLayers.GetLayers());

                        string material;
                        double thickness;

                        foreach (CompoundStructureLayer layer in layers)
                        {
                            string testLayer = layer.Function.ToString();

                            double weight = 0;

                            if (testLayer == "Structure")
                            {
                                Material structuralLayerDeck = doc.GetElement(layer.MaterialId) as Material;

                                material = structuralLayerDeck.Name;
                                thickness = RoundToSignificantDigits.RoundDigits(ImperialToMetricConverter.ConvertFromFeetToMeters(layer.Width), 4);

                                Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                                decks.AddDeck(deck);

                            }

                        }
                    }

                    else if (element is FootPrintRoof)
                    {
                        // Creates the TypeId
                        int typeID = element.Id.IntegerValue;


                        // Maps the area of the deck
                        double area1 = ImperialToMetricConverter.ConvertFromSquaredFeetToSquaredMeters(element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                        double area = RoundToSignificantDigits.RoundDigits(area1, 4);
                        //string material = carsten.FloorType.LookupParameter("Structural Material").AsValueString();

                        RoofType carsten = doc.GetElement(element.GetTypeId()) as RoofType;

                        //if material consists of more than two layers
                        CompoundStructure deckLayers = carsten.GetCompoundStructure();

                        List<CompoundStructureLayer> layers = new List<CompoundStructureLayer>(deckLayers.GetLayers());

                        string material;
                        string quality;
                        double thickness;

                        foreach (CompoundStructureLayer layer in layers)
                        {
                            string testLayer = layer.Function.ToString();

                            //weight is not relevnt for deck
                            double weight = 0;

                            if (testLayer == "Structure")
                            {
                                
                                Material structuralLayerDeck = doc.GetElement(layer.MaterialId) as Material;

                                material = structuralLayerDeck.Name;
                                quality = structuralLayerDeck.Name;
                                thickness = RoundToSignificantDigits.RoundDigits(ImperialToMetricConverter.ConvertFromFeetToMeters(layer.Width), 3);

                                Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                                decks.AddDeck(deck);

                            }

                        }
                    }
                }

            }

            Foundations foundations = new Foundations();
            // Assigns the revit parameters to the Foundation constructor
            foreach (Element element in listOfAllFoundation)
            {
                var test = element.GetType().Name;
                if (test == "WallFoundation")
                {
                    int typeID = element.Id.IntegerValue;

                    string quality = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();

                    double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                    double volume = Math.Round(RoundToSignificantDigits.RoundDigits(volume1, 4), 5);

                    string material = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();

                    double weight = 0;
                    if (material.Contains("Steel"))
                    {
                        weight = Math.Round(RoundToSignificantDigits.RoundDigits(WeightOfSteel.Convert(volume), 4), 5);
                    }

                    Foundation foundation = new Foundation(typeID, material, quality, volume, weight);
                    foundations.AddFoundation(foundation);

                }
                else if (test == "Floor")
                {
                    int typeID = element.Id.IntegerValue;

                    string quality = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();

                    double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                    double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);

                    FloorType carsten = doc.GetElement(element.GetTypeId()) as FloorType;

                    //if material consists of more than two layers
                    CompoundStructure deckLayers = carsten.GetCompoundStructure();

                    List<CompoundStructureLayer> layers = new List<CompoundStructureLayer>(deckLayers.GetLayers());

                    string material;

                    foreach (CompoundStructureLayer layer in layers)
                    {
                        string testLayer = layer.Function.ToString();

                        if (testLayer == "Structure")
                        {
                            Material structuralLayerDeck = doc.GetElement(layer.MaterialId) as Material;

                            material = structuralLayerDeck.Name;

                            double weight = 0;
                            if (material.Contains("Steel"))
                            {
                                weight = RoundToSignificantDigits.RoundDigits(WeightOfSteel.Convert(volume), 4);
                            }

                            Foundation foundation = new Foundation(typeID, material, quality, volume, weight);
                            foundations.AddFoundation(foundation);

                        }

                    }

                }
                else if (test == "FamilyInstance")
                {

                    int typeID = element.Id.IntegerValue;
                    
                    var cast = (FamilyInstance)element;
                    string material = cast.StructuralMaterialType.ToString();
                    //string material = doc.GetElement(element.GetTypeId()).LookupParameter("Structural Material").AsValueString();
                    Family family = doc.GetElement(cast.GetTypeId()) as Family;

                    string testQuality = Convert.ToString(doc.GetElement(cast.StructuralMaterialId) as Material);
                    string quality;

                    if (testQuality == "")
                    {
                        quality = "Not defined";

                    }
                    else
                    {
                        var quality1 = doc.GetElement(cast.StructuralMaterialId) as Material;
                        quality = quality1.Name.ToString();

                    }

                    double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());
                    double volume = RoundToSignificantDigits.RoundDigits(volume1, 3);

                    double weight = 0;
                    if (material.Contains("Steel"))
                    {
                        weight = RoundToSignificantDigits.RoundDigits(WeightOfSteel.Convert(volume), 4);
                    }

                    Foundation foundation = new Foundation(typeID, material, quality, volume, weight);
                    foundations.AddFoundation(foundation);
                }
                

            }

            Reinforcements reinforcements = new Reinforcements();
            // Assigns the revit parameters to the Beam constructor
            foreach (Element element in listOfAllReinforcement)
            {

                // Creates the TypeId

                int typeID = element.Id.IntegerValue;

                //Maps the material of the beam
                string material = "Reinforcement";

                string quality = doc.GetElement(element.GetTypeId()).Name;
                
                double volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(element.LookupParameter("Reinforcement Volume").AsDouble());
                //Alternative way to get volume
                //var volume1 = ImperialToMetricConverter.ConvertFromCubicFeetToCubicMeters(cast.LookupParameter("Volume").AsDouble());
                double volume = Math.Round(RoundToSignificantDigits.RoundDigits(volume1, 4),5);

                double weight = Math.Round(RoundToSignificantDigits.RoundDigits(WeightOfSteel.Convert(volume), 4),5);

                if (element is RebarInSystem)
                {
                    var rebar = (RebarInSystem)element;


                    //List<Reference> references = new List<Reference>();
                    ElementId hostId = rebar.GetHostId();
                    //var hostCategory = hostId.GetCategory()
                    string hostCategory = doc.GetElement(hostId).Category.Name;


                    //area and thickness is not relevant for reinforcement
                    double area = 0;
                    double thickness = 0;

                    if (hostCategory == "Parts")
                    {
                        ExteriorWall exteriorWall = new ExteriorWall(typeID, material, quality, area, thickness, weight);
                        exteriorWalls.AddExteriorWall(exteriorWall);
                    }
                    else if (hostCategory == "Floors")
                    {
                        Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                        decks.AddDeck(deck);
                    }
                    else if (hostCategory == "Structural Foundations")
                    {
                        Foundation foundation = new Foundation(typeID, material, quality, volume, weight);
                        foundations.AddFoundation(foundation);
                    }
                    else if (hostCategory == "Walls")
                    {
                        ExteriorWall exteriorWall = new ExteriorWall(typeID, material, quality, area, thickness, weight);
                        exteriorWalls.AddExteriorWall(exteriorWall);
                    }
                }
                else if (element is Rebar)
                {
                    var rebar = (Rebar)element;


                    //List<Reference> references = new List<Reference>();
                    ElementId hostId = rebar.GetHostId();
                    //var hostCategory = hostId.GetCategory()
                    string hostCategory = doc.GetElement(hostId).Category.Name;


                    //area and thickness is not relevant for reinforcement
                    double area = 0;
                    double thickness = 0;

                    if (hostCategory == "Parts")
                    {
                        ExteriorWall exteriorWall = new ExteriorWall(typeID, material, quality, area, thickness, weight);
                        exteriorWalls.AddExteriorWall(exteriorWall);
                    }
                    else if (hostCategory == "Floors")
                    {
                        Deck deck = new Deck(typeID, material, quality, area, thickness, weight);
                        decks.AddDeck(deck);
                    }
                    else if (hostCategory == "Structural Foundations")
                    {
                        Foundation foundation = new Foundation(typeID, material, quality, volume, weight);
                        foundations.AddFoundation(foundation);
                    }
                    else if (hostCategory == "Walls")
                    {
                        ExteriorWall exteriorWall = new ExteriorWall(typeID, material, quality, area, thickness, weight);
                        exteriorWalls.AddExteriorWall(exteriorWall);
                    }
                }

            }

            // Add all structural elements to a Dictionary of Structuralelements
            StructuralElements structuralElements = new StructuralElements();

            // Lav breakpoint og kopier JSON filen.
            JsonConvert.SerializeObject(structuralElements.CreateDictionary(beams, columns, decks, exteriorWalls, interiorWalls, foundations), (Formatting)1);

            File.WriteAllText(@"C:\Users\camil\Documents\Structuralelements_Json", JsonConvert.SerializeObject(structuralElements.CreateDictionary(beams, columns, decks, exteriorWalls, interiorWalls, foundations)));

            return Result.Succeeded;

        }


    }
}



    


