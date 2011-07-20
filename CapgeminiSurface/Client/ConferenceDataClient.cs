using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapgeminiSurface.ConferenceDataService;

namespace CapgeminiSurface.Client
{
    public class ConferenceDataClient
    {
        NdcAgendaContainer container;
        Agenda agenda;

        public int Day = 1;

        public ConferenceDataClient()
        {
            Init();
        }

        private void Init()
        {
            if (agenda != null) return;
            container = new NdcAgendaContainer(new Uri("http://81.27.45.120:810/ConferenceData.svc"));
            agenda = container.Agendas
                                .Expand("Speakers,Sessions/Speakers")
                                .Where(a => a.AgendaId == "NDC2011")
                                .OrderByDescending(a => a.Version)
                                .FirstOrDefault();
        }

        public List<ConferenceSession> GetSessions(int track)
        {
            List<ConferenceSession> result = new List<ConferenceSession>();

            Init();

            foreach (var session in agenda.Sessions.Where(s => s.Day == Day && s.Track == track).OrderBy(s => s.StartHour))
            {
                result.Add(new ConferenceSession() { Title = session.Title, StartHour = session.StartHour });
            }
            return result;
        }
    }
}
