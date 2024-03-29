using System;
using System.Windows.Forms;
using System.Collections.Generic;
using mwSkills.Dialogs;

namespace ToDemo.Data;

public class TestMember : IListViewData
{
    public int ID { get; set; }
    public DateTime RegTime { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Remark { get; set; }

    public ListViewItem ToLsvItem()
    {
        string[] contents = { $"{ID:D08}", Name, $"{Age}", RegTime.ToString("yyyy/MM/dd HH:mm:ss"), Remark };
        return new ListViewItem(contents);
    }

    public override string ToString()
    {
        return $"#{ID} Name:{Name} Age:{Age} (registry @{RegTime.ToString("MM/dd")}) | Remark:{Remark}";
    }
    
    public static ColumnHeader[] ColumnHeaders { get; } = {
        new ColumnHeader { TextAlign = HorizontalAlignment.Center, Name = "編號", Width = 50 },
        new ColumnHeader { TextAlign = HorizontalAlignment.Center, Name = "名字", Width = 150 },
        new ColumnHeader { TextAlign = HorizontalAlignment.Center, Name = "年齡", Width = 30 },
        new ColumnHeader { TextAlign = HorizontalAlignment.Center, Name = "註冊時間", Width = 200 },
        new ColumnHeader { TextAlign = HorizontalAlignment.Center, Name = "備註", Width = 200 }
    };

    public static TestMember GenerateRandomMember(int? id = null, Random random = null)
    {
        if (random == null)
            random = new Random(Environment.TickCount64.GetHashCode());
        int randomCode = random.Next();
        
        return new TestMember {
            ID = (id.HasValue ? id.Value : 0),
            RegTime = DateTime.Now,
            Name = $"User_{random.NextInt64():X08}",
            Age = random.Next(0, 99),
            Remark = "/* GenerateRandomMember() made */"
        };
    }
    public static List<TestMember> GenerateRandomMembers(int amount, int startId = 0)
    {
        if (amount < 1)
            throw new ArgumentException("The argument 'amount' must be greater than 0.");
        Random r = new Random((Environment.TickCount64/amount + startId).GetHashCode());
        List<TestMember> members = new List<TestMember>();
        int i = 0;
        do { members.Add(GenerateRandomMember(startId + i, r)); } while (++i < amount);

        return members;
    }
}