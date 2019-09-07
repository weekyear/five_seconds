using Five_Seconds.Services;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Five_Seconds.Models
{
    [Table("Mission")]
    public class Mission : INotifyPropertyChanged, IObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, NotNull, AutoIncrement]
        public int Id { get; set; }
        public int AlarmId { get; set; }

        public string Name { get; set; }
        public double Percentage { get; set; }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive == value) return;
                if (IsIntiated && value)
                {
                    App.Service.SaveMission(this);
                }

                if (!IsIntiated && Id == MissionService.FinalMissionId)
                {
                    IsIntiated = true;
                }
                isActive = value;
            }
        }

        [Ignore]
        private bool IsIntiated { get; set; }

        [OneToMany]
        public List<Record> Records { get; set; } = new List<Record>();

        [OneToOne]
        public Alarm Alarm { get; set; } = new Alarm();

        public Mission() { }

        public Mission (Mission original)
        {
            Id = original.Id;
            Name = original.Name;
            Alarm = original.Alarm;
            Percentage = original.Percentage;
            IsActive = original.IsActive;
        }
    }
}