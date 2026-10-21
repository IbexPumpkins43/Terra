namespace Terra.Forms
{
    internal class FormContext : ApplicationContext
    {
        public void SwitchForm(Form current, Form next)
        {
            next.FormClosed += (sender, e) => this.ExitThread();

            this.MainForm = next;
            this.MainForm.Show();

            current.Close();
        }
    }
}
