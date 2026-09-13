using System;
using System.Collections.Generic;
using System.Linq;
using Outposts;
using RimWorld;
using VEF.AnimalBehaviours;
using Verse;

namespace VOEE;

public class Outpost_Ranching : Outpost_ChooseResult
{
    [PostToSetings("Outposts.Settings.BodySize", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 2f)]
    public readonly float BodySize = 1f;

    [PostToSetings("Outposts.Settings.Count", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 5f)]
    public readonly float CountMultiplier = 1f;

    [PostToSetings("Outposts.Settings.Egg", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public readonly float Egg = 0.5f;

    [PostToSetings("Outposts.Settings.HungerRate", PostToSetingsAttribute.DrawMode.Percentage, 1f, 0.01f, 2f)]
    public readonly float HungerRate = 1f;

    [PostToSetings("Outposts.Settings.Leather", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public readonly float Leather = 0.5f;

    [PostToSetings("Outposts.Settings.Meat", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public readonly float Meat = 0.5f;

    [PostToSetings("Outposts.Settings.Milk", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public readonly float Milk = 0.5f;

    [PostToSetings("Outposts.Settings.OtherProduct", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public readonly float Other = 0.5f;

    [PostToSetings("Outposts.Settings.Production", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 5f)]
    public readonly float ProductionMultiplier = 0.5f;

    [PostToSetings("Outposts.Settings.Wool", PostToSetingsAttribute.DrawMode.Percentage, 0.5f, 0.01f, 2f)]
    public readonly float Wool = 0.5f;

    public override List<ResultOption> ResultOptions
    {
        get
        {
            var resultOption = base.ResultOptions.FirstOrDefault();
            if (resultOption?.Thing == null)
            {
                return [];
            }

            var thing = resultOption.Thing;
            var race = thing.race;
            if (race == null)
            {
                return [];
            }

            var offspringFactor = GetOffspringFactor(thing, race);
            var growthDurationInDays = GetGrowthDurationInDays(thing, race);
            var resultOptions = new List<ResultOption>
            {
                new()
                {
                    Thing = race.leatherDef ?? ThingDefOf.Leather_Plain,
                    BaseAmount = Math.Max((int)(ProductionMultiplier * Leather *
                                                thing.GetStatValueAbstract(StatDefOf.LeatherAmount) *
                                                resultOption.BaseAmount *
                                                offspringFactor * 0.5 *
                                                15 /
                                                growthDurationInDays), 1)
                },
                new()
                {
                    Thing = race.meatDef ?? ThingDefOf.Cow.race.meatDef ?? ThingDefOf.Meat_Human,
                    BaseAmount = Math.Max((int)(ProductionMultiplier * Meat *
                                                thing.GetStatValueAbstract(StatDefOf.MeatAmount) *
                                                resultOption.BaseAmount *
                                                offspringFactor * 0.5 *
                                                15 /
                                                growthDurationInDays), 1)
                }
            };
            var milkable = resultOption.Thing.GetCompProperties<CompProperties_Milkable>();
            if (milkable != null)
            {
                resultOptions.Add(
                    new ResultOption
                    {
                        Thing = milkable.milkDef,
                        BaseAmount = Math.Max((int)(ProductionMultiplier * Milk * resultOption.BaseAmount *
                                                    (milkable.milkFemaleOnly ? 0.5 : 1) * milkable.milkAmount /
                                                    milkable.milkIntervalDays *
                                                    15),
                            1)
                    }
                );
            }

            var shearable = resultOption.Thing.GetCompProperties<CompProperties_Shearable>();
            if (shearable != null)
            {
                resultOptions.Add(
                    new ResultOption
                    {
                        Thing = shearable.woolDef,
                        BaseAmount = Math.Max((int)(ProductionMultiplier * Wool * resultOption.BaseAmount *
                            shearable.woolAmount /
                            shearable.shearIntervalDays * 15), 1)
                    }
                );
            }

            var eggLayer = resultOption.Thing.GetCompProperties<CompProperties_EggLayer>();
            if (eggLayer is { eggProgressUnfertilizedMax: 1 })
            {
                resultOptions.Add(
                    new ResultOption
                    {
                        Thing = eggLayer.eggUnfertilizedDef,
                        BaseAmount = Math.Max((int)(ProductionMultiplier * Egg * resultOption.BaseAmount *
                            (eggLayer.eggLayFemaleOnly ? 0.5 : 1) * eggLayer.eggCountRange.Average /
                            eggLayer.eggLayIntervalDays * 15), 1)
                    }
                );
            }

            var animalProduct = resultOption.Thing.GetCompProperties<CompProperties_AnimalProduct>();
            if (animalProduct?.resourceDef != null)
            {
                resultOptions.Add(
                    new ResultOption
                    {
                        Thing = animalProduct.resourceDef,
                        BaseAmount = Math.Max((int)(ProductionMultiplier * Other * resultOption.BaseAmount *
                            animalProduct.resourceAmount / animalProduct.gatheringIntervalDays * 15), 1)
                    }
                );
            }

            return resultOptions;
        }
    }

    public override IEnumerable<ResultOption> GetExtraOptions()
    {
        var animalsSkillTotal = CapablePawns.Sum(p => p.skills.GetSkill(SkillDefOf.Animals).Level);
        return DefDatabase<PawnKindDef>.AllDefs
            .Where(pkd => pkd.race?.tradeTags != null && pkd.race.tradeTags.Contains("AnimalFarm") ||
                          pkd.defName == "Boomalope")
            .Select(pkd => new ResultOption
            {
                Thing = pkd.race,
                BaseAmount = Math.Max((int)Math.Ceiling(CountMultiplier /
                                                        ((HungerRate * pkd.race.race.baseHungerRate) +
                                                         (BodySize * pkd.race.race.baseBodySize)) *
                                                        animalsSkillTotal), 1)
            });
    }

    private static float GetOffspringFactor(ThingDef thing, RaceProperties race)
    {
        var eggLayer = thing.GetCompProperties<CompProperties_EggLayer>();
        if (eggLayer != null)
        {
            return eggLayer.eggCountRange.Average;
        }

        return race.litterSizeCurve == null ? 1f : Rand.ByCurveAverage(race.litterSizeCurve);
    }

    private static float GetGrowthDurationInDays(ThingDef thing, RaceProperties race)
    {
        var eggLayer = thing.GetCompProperties<CompProperties_EggLayer>();
        var gestationOrHatchingDays = race?.gestationPeriodDays ??
                                      (eggLayer == null
                                          ? 0f
                                          : eggLayer.eggFertilizedDef
                                              .GetCompProperties<CompProperties_Hatcher>()
                                              .hatcherDaystoHatch);

        var lifeStageAges = race?.lifeStageAges;
        if (lifeStageAges == null)
        {
            return gestationOrHatchingDays;
        }

        var adulthoodDays = lifeStageAges[^1].minAge * 60f;

        return gestationOrHatchingDays + adulthoodDays;
    }
}