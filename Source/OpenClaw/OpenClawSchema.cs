namespace OpenClaw
{
    public static class OpenClawSchema
    {
        public static string Schema = @"{
  \"version\": 1,
  \"actions\": [
    {\"action\":\"set_priority\",\"required\":[\"pawn\",\"work\",\"level\"],\"example\":{\"pawn\":\"Name\",\"work\":\"Construction\",\"level\":1}},
    {\"action\":\"queue_build\",\"required\":[\"thing\",\"pos\"],\"example\":{\"thing\":\"Wall\",\"pos\":[12,0,18]}},
    {\"action\":\"draft\",\"required\":[\"pawn\"],\"example\":{\"pawn\":\"Name\"}},
    {\"action\":\"undraft\",\"required\":[\"pawn\"],\"example\":{\"pawn\":\"Name\"}},
    {\"action\":\"set_zone\",\"required\":[\"pawn\",\"zone\"],\"example\":{\"pawn\":\"Name\",\"zone\":\"Home\"}},

    {\"action\":\"create_bill\",\"required\":[\"table\",\"recipe\"],\"example\":{\"table\":\"ElectricStove\",\"recipe\":\"CookMealSimple\"}},
    {\"action\":\"set_bill_count\",\"required\":[\"table\",\"recipe\",\"count\"],\"example\":{\"table\":\"ElectricStove\",\"recipe\":\"CookMealSimple\",\"count\":10}},
    {\"action\":\"set_bill_pause\",\"required\":[\"table\",\"recipe\",\"paused\"],\"example\":{\"table\":\"ElectricStove\",\"recipe\":\"CookMealSimple\",\"paused\":true}},
    {\"action\":\"set_bill_repeat_mode\",\"required\":[\"table\",\"recipe\",\"repeat_mode\"],\"example\":{\"table\":\"ElectricStove\",\"recipe\":\"CookMealSimple\",\"repeat_mode\":\"forever\"}},
    {\"action\":\"set_bill_target\",\"required\":[\"table\",\"recipe\",\"target\"],\"example\":{\"table\":\"ElectricStove\",\"recipe\":\"CookMealSimple\",\"target\":20}},
    {\"action\":\"set_bill_skill_range\",\"required\":[\"table\",\"recipe\",\"min_skill\",\"max_skill\"],\"example\":{\"table\":\"ElectricStove\",\"recipe\":\"CookMealSimple\",\"min_skill\":4,\"max_skill\":12}},
    {\"action\":\"set_bill_ingredients\",\"required\":[\"table\",\"recipe\",\"ingredients\"],\"example\":{\"table\":\"ElectricStove\",\"recipe\":\"CookMealSimple\",\"ingredients\":[\"Meat\",\"Rice\"]}},

    {\"action\":\"set_schedule\",\"required\":[\"pawn\",\"schedule\"],\"example\":{\"pawn\":\"Name\",\"schedule\":\"work,work,work,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,anything,sleep,sleep,sleep,sleep\"}},

    {\"action\":\"move_to\",\"required\":[\"pawn\",\"target_pos\"],\"example\":{\"pawn\":\"Name\",\"target_pos\":[10,0,12]}},
    {\"action\":\"attack\",\"required\":[\"pawn\",\"target\"],\"example\":{\"pawn\":\"Name\",\"target\":\"Raider1\"}},
    {\"action\":\"attack_ranged\",\"required\":[\"pawn\",\"target\"],\"example\":{\"pawn\":\"Name\",\"target\":\"Raider1\"}},
    {\"action\":\"attack_pos\",\"required\":[\"pawn\",\"target_pos\"],\"example\":{\"pawn\":\"Name\",\"target_pos\":[14,0,18]}},
    {\"action\":\"attack_thing\",\"required\":[\"pawn\",\"target\"],\"example\":{\"pawn\":\"Name\",\"target\":\"Turret\"}},
    {\"action\":\"hold_position\",\"required\":[\"pawn\"],\"example\":{\"pawn\":\"Name\"}},
    {\"action\":\"flee\",\"required\":[\"pawn\",\"target\"],\"example\":{\"pawn\":\"Name\",\"target\":\"Raider1\"}},
    {\"action\":\"retreat\",\"required\":[\"pawn\",\"target_pos\"],\"example\":{\"pawn\":\"Name\",\"target_pos\":[5,0,6]}},

    {\"action\":\"group_draft\",\"required\":[\"pawns\"],\"example\":{\"pawns\":[\"Name1\",\"Name2\"]}},
    {\"action\":\"group_undraft\",\"required\":[\"pawns\"],\"example\":{\"pawns\":[\"Name1\",\"Name2\"]}},
    {\"action\":\"group_move\",\"required\":[\"pawns\",\"target_pos\"],\"example\":{\"pawns\":[\"Name1\",\"Name2\"],\"target_pos\":[10,0,12]}},
    {\"action\":\"group_attack\",\"required\":[\"pawns\",\"target\"],\"example\":{\"pawns\":[\"Name1\",\"Name2\"],\"target\":\"Raider1\"}}
  ]
}";
    }
}
