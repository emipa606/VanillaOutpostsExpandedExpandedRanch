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

            var race = resultOption.Thing.race;
            if (race == null)
            {
                return [];
            }

            var resultOptions = new List<ResultOption>
            {
                new()
                {
                    Thing = race.leatherDef ?? ThingDefOf.Leather_Plain,
                    BaseAmount = Math.Max((int)(ProductionMultiplier * Leather *
                                                resultOption.Thing.GetStatValueAbstract(StatDefOf.LeatherAmount) *
                                                resultOption.BaseAmount *
                                                (resultOption.Thing.HasComp(typeof(CompEggLayer))
                                                    ? resultOption.Thing.GetCompProperties<CompProperties_EggLayer>()
                                                        .eggCountRange
                                                        .Average
                                                    : resultOption.Thing.race.litterSizeCurve == null
                                                        ? 1
                                                        : Rand.ByCurveAverage(resultOption.Thing.race
                                                            .litterSizeCurve)) * 0.5 *
                                                15 /
                                                ((resultOption.Thing.race?.gestationPeriodDays ??
                                                  (resultOption.Thing.GetCompProperties<CompProperties_EggLayer>() ==
                                                   null
                                                      ? 0
                                                      : resultOption.Thing.GetCompProperties<CompProperties_EggLayer>()
                                                          .eggFertilizedDef
                                                          .GetCompProperties<CompProperties_Hatcher>()
                                                          .hatcherDaystoHatch)) +
                                                 (race.lifeStageAges.Last().minAge * 60))), 1)
                },
                new()
                {
                    Thing = race.meatDef ?? ThingDefOf.Cow.race.meatDef ?? ThingDefOf.Meat_Human,
                    BaseAmount = Math.Max((int)(ProductionMultiplier * Meat *
                                                resultOption.Thing.GetStatValueAbstract(StatDefOf.MeatAmount) *
                                                resultOption.BaseAmount *
                                                (resultOption.Thing.HasComp(typeof(CompEggLayer))
                                                    ? resultOption.Thing.GetCompProperties<CompProperties_EggLayer>()
                                                        .eggCountRange
                                                        .Average
                                                    : race.litterSizeCurve == null
                                                        ? 1
                                                        : Rand.ByCurveAverage(race.litterSizeCurve)) * 0.5 *
                                                15 /
                                                ((resultOption.Thing.race?.gestationPeriodDays ??
                                                  (resultOption.Thing.GetCompProperties<CompProperties_EggLayer>() ==
                                                   null
                                                      ? 0
                                                      : resultOption.Thing.GetCompProperties<CompProperties_EggLayer>()
                                                          .eggFertilizedDef
                                                          .GetCompProperties<CompProperties_Hatcher>()
                                                          .hatcherDaystoHatch)) +
                                                 (race.lifeStageAges.Last().minAge * 60))), 1)
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
        var animalsSkillTotal = CapablePawns.ToList().Sum(p => p.skills.GetSkill(SkillDefOf.Animals).Level);
        return from pkd in from pkd in DefDatabase<PawnKindDef>.AllDefs
                where (pkd.race?.tradeTags != null && pkd.race.tradeTags.Contains("AnimalFarm")) ||
                      pkd.defName == "Boomalope"
                select pkd
            select new ResultOption
            {
                Thing = pkd.race,
                BaseAmount = Math.Max((int)Math.Ceiling(CountMultiplier /
                                                        ((HungerRate * pkd.race.race.baseHungerRate) +
                                                         (BodySize * pkd.race.race.baseBodySize)) *
                                                        animalsSkillTotal), 1)
            };
    }
}