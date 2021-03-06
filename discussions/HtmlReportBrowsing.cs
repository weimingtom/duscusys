﻿using Discussions.view;
using Reporter.pdf;
namespace Discussions
{
    class HtmlReportBrowsing
    {
        public static void startResultViewer()
        {
            if (SessionInfo.Get().discussion == null)
                MessageDlg.Show("No default discussion");
            else
            {
                if (SessionInfo.Get().person.Session == null)
                {
                    MessageDlg.Show(
                        string.Format(
                            "The function requires experimental mode (session)!",
                            SessionInfo.Get().person.Name));
                    return;
                }
                var tsd = new TopicSelectionDlg(SessionInfo.Get().discussion);
                tsd.ShowDialog();
                if (tsd.topic == null)
                    return;

                if (tsd.Html)
                {
                    var reportUrl = string.Format(
                        "http://{0}/discsvc/report?discussionId={1}&topicId={2}&sessionId={3}",
                        ConfigManager.ServiceServer,
                        SessionInfo.Get().discussion.Id,
                        tsd.topic.Id,
                        SessionInfo.Get().person.Session.Id);

                    try
                    {
                        System.Diagnostics.Process.Start("chrome", reportUrl);
                    }
                    catch 
                    {
                        System.Diagnostics.Process.Start(reportUrl);                    
                    }
                    //var browser = WebkitBrowserWindow.Instance(reportUrl, tsd.topic.Id);
                    //browser.Show();
                    //browser.Activate();
                }
                else
                {
                    BusyWndSingleton.Show("Exporting, please wait...");
                    try
                    {
                        (new PdfReportDriver()).Run(SessionInfo.Get().person.Session,
                                                    tsd.topic,
                                                    SessionInfo.Get().discussion,
                                                    SessionInfo.Get().person);
                    }
                    finally
                    {
                        BusyWndSingleton.Hide();
                    }
                }
            }
        }

        public static void startHtml5TopicReport(int topicId)
        {
            if (SessionInfo.Get().discussion == null)
            {
                MessageDlg.Show("No default discussion");
                return;
            }

            if (SessionInfo.Get().person.Session == null)
            {
                MessageDlg.Show(
                    string.Format(
                        "The function requires experimental mode (session)!",
                        SessionInfo.Get().person.Name));
                return;
            }

            var reportUrl = string.Format(
                "http://{0}/discsvc/report?discussionId={1}&topicId={2}&sessionId={3}",
                ConfigManager.ServiceServer,
                SessionInfo.Get().discussion.Id,
                topicId,
                SessionInfo.Get().person.Session.Id);

            try
            {
                System.Diagnostics.Process.Start("chrome", reportUrl);
            }
            catch
            {
                System.Diagnostics.Process.Start(reportUrl);
            }
            //var browser = WebkitBrowserWindow.Instance(reportUrl, topicId);
            //browser.Show();
            //browser.Activate();
        }
    }
}
