/// <summary> 
/// Desc: 游戏配置
/// Excel: GameConfigSetting.xlsx
/// Author: ConfigTool
/// </summary>
public class GameConfigSettingLO
{
    /// <summary> 
    /// 主键
    /// </summary>
    public int id;

    /// <summary> 
    /// 键
    /// </summary>
    public string key;

    /// <summary> 
    /// 值
    /// </summary>
    public string value;

    /// <summary> 
    /// 测试值
    /// </summary>
    public string TestValue;

    /// <summary> 
    /// int64
    /// </summary>
    public long longTest;

    /// <summary> 
    /// 多语言测试
    /// </summary>
    public int name;

    public string nameLang => LanguageManager.Instance.GetLanguageText(name);


}