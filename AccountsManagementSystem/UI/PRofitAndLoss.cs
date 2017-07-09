﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AccountsManagementSystem.DbGateway;
using AccountsManagementSystem.Models;

namespace AccountsManagementSystem.UI
{
    public partial class PRofitAndLoss : Form
    {
        private Dictionary<int, string> accountTypeDictionary=new Dictionary<int, string>();
        private List<SubAccountTypes> SubAccountTypesList =new List<SubAccountTypes>();
        private List<Groups> GroupList = new List<Groups>();
        private SqlDataReader rdr;
        private SqlCommand cmd;
        private SqlConnection con;
        ConnectionString cs=new ConnectionString();
        private DataGridViewRow dr;
        private int ExpenseSid, EGId,Lid;
        private int RevenueSid,RGId;
        private decimal balance,totalRevenue=0.0m,totalExpense=0.0m,profitorLoss=0.0m;
        public PRofitAndLoss()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Please Select Account Type");
            }

            else if (string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                MessageBox.Show("Please Select Group");
            }
            else if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please Select Ledger");
            }
            else
            {
                totalRevenue = totalRevenue + balance;
                textBox3.Text = totalRevenue.ToString();
                dataGridView3.Rows.Add(Lid, textBox1.Text, balance, RevenueSid, RGId);
                ClearRevenues();
                CalculatePNL();
            }
           
        }

        private void CalculatePNL()
        {
            if (totalRevenue > totalExpense)
            {
                label7.Text = "Profit";
                textBox5.Text = (totalRevenue - totalExpense).ToString();
            }
            else if (totalRevenue < totalExpense)
            {
                label7.Text = "Loss";
                textBox5.Text = (totalExpense - totalRevenue).ToString();
            }
            else
            {
                label7.Text = "No Profit or  Loss";
                textBox5.Text = "0";
            }
        }

        private void ClearExpenses()
        {
            dataGridView2.Rows.Remove(dr);
            comboBox3.SelectedIndexChanged -= comboBox3_SelectedIndexChanged;
            comboBox3.SelectedIndex = -1;
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
            comboBox4.SelectedIndexChanged -= comboBox4_SelectedIndexChanged;
            comboBox4.SelectedIndex = -1;
            comboBox4.SelectedIndexChanged += comboBox4_SelectedIndexChanged;
            textBox2.Clear();
        }
        private void ClearRevenues()
        {
            dataGridView1.Rows.Remove(dr);
            comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
            comboBox1.SelectedIndex = -1;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged -= comboBox2_SelectedIndexChanged;
            comboBox2.SelectedIndex = -1;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            textBox1.Clear();
        }

        private void PRofitAndLoss_Load(object sender, EventArgs e)
        {
            LoadSubAccountTypes();
            LoadGroups();
            LoadRevenueAccountTypes();
            LoadExpenseAccountTypes();
            LoadGridOne();
            LoadGridTwo();
        }

        private void LoadExpenseAccountTypes()
        {
            var expenseaccounts = from SubAccountTypes in SubAccountTypesList
                where SubAccountTypes.AccountType == "Expense"
                select SubAccountTypes.SName;
            foreach (var subaccounttypes in expenseaccounts)
            {
                comboBox4.Items.Add(subaccounttypes);
            }
        }

        private void LoadRevenueAccountTypes()
        {
            var revenueaccounts = from SubAccountTypes in SubAccountTypesList
                where SubAccountTypes.AccountType == "Revenue"
                select SubAccountTypes.SName;
            foreach (var subaccounttypes in revenueaccounts)
            {
                comboBox1.Items.Add(subaccounttypes);
            }
        }

        private void LoadGroups()
        {
            con = new SqlConnection(cs.DBConn);
            cmd = new SqlCommand();
            cmd.Connection = con;
            string query = "SELECT Groups.GId, Groups.GroupName, SubAccountTypes.SubAccountType FROM Groups INNER JOIN SubAccountTypes ON Groups.SATId = SubAccountTypes.SATId";
            con.Open();
            cmd.CommandText = query;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Groups grup = new Groups();
                grup.GId = Convert.ToInt32(rdr[0]);
                grup.GroupName = rdr[1].ToString();
                grup.Stype = rdr[2].ToString();
                GroupList.Add(grup);
            }
        }
        private void LoadGridOne()
        {
            con = new SqlConnection(cs.DBConn);
            cmd = new SqlCommand();
            cmd.Connection = con;
            string query = "SELECT BalanceFiscal.LId, Ledger.LedgerName, BalanceFiscal.Balance FROM Ledger INNER JOIN BalanceFiscal ON Ledger.LedgerId = BalanceFiscal.LedgerId INNER JOIN AGRel ON Ledger.AGRelId = AGRel.AGRelId WHERE AGRel.AccountType = 'Revenue' and BalanceFiscal.FiscalId=17";
            con.Open();
            cmd.CommandText = query;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2]);
            }
        }
        private void LoadGridTwo()
        {
            con = new SqlConnection(cs.DBConn);
            cmd = new SqlCommand();
            cmd.Connection = con;
            string query = "SELECT BalanceFiscal.LId, Ledger.LedgerName, BalanceFiscal.Balance FROM Ledger INNER JOIN BalanceFiscal ON Ledger.LedgerId = BalanceFiscal.LedgerId INNER JOIN AGRel ON Ledger.AGRelId = AGRel.AGRelId WHERE AGRel.AccountType = 'Expense' and BalanceFiscal.FiscalId=17";
            con.Open();
            cmd.CommandText = query;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2]);
            }
        }
        private void LoadSubAccountTypes()
        {
            con = new SqlConnection(cs.DBConn);
            cmd = new SqlCommand();
            cmd.Connection = con;
            string query = "SELECT SATId,SubAccountType,AGRel.AccountType FROM SubAccountTypes inner join AGRel on SubAccountTypes.AGRelId=AGRel.AGRelId";
            con.Open();
            cmd.CommandText = query;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                SubAccountTypes SubAccountType = new SubAccountTypes();
                SubAccountType.SId = Convert.ToInt32(rdr[0]);
                SubAccountType.SName = rdr[1].ToString();
                SubAccountType.AccountType = rdr[2].ToString();
                SubAccountTypesList.Add(SubAccountType);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            var revenueSid = from SubAccountTypes in SubAccountTypesList
                where SubAccountTypes.SName == comboBox1.Text
                select SubAccountTypes;
            RevenueSid = revenueSid.FirstOrDefault().SId;
            var groups = from Groups in GroupList
                where Groups.Stype == comboBox1.Text
                select Groups.GroupName;
            foreach (string grGroup in groups)
            {
                comboBox2.Items.Add(grGroup);
            }

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            var expenseSid = from SubAccountTypes in SubAccountTypesList
                where SubAccountTypes.SName == comboBox4.Text
                select SubAccountTypes;
            ExpenseSid = expenseSid.FirstOrDefault().SId;
            var groups = from Groups in GroupList
                where Groups.Stype == comboBox4.Text
                select Groups.GroupName;
            foreach (string grGroup in groups)
            {
                comboBox3.Items.Add(grGroup);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var rGid = from grups in GroupList
                where grups.GroupName == comboBox2.Text
                select grups;
            RGId = rGid.FirstOrDefault().GId;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var eGid = from grups in GroupList
                where grups.GroupName == comboBox3.Text
                select grups;
            EGId = eGid.FirstOrDefault().GId;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
 dr = dataGridView1.SelectedRows[0];
            Lid = Convert.ToInt32(dr.Cells[0].Value);
            textBox1.Text = dr.Cells[1].Value.ToString();
            balance = Convert.ToDecimal(dr.Cells[2].Value);

        }

        private void dataGridView2_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dr = dataGridView2.SelectedRows[0];
            Lid = Convert.ToInt32(dr.Cells[0].Value);
            textBox2.Text = dr.Cells[1].Value.ToString();
            balance = Convert.ToDecimal(dr.Cells[2].Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBox3.Text))
            {
                MessageBox.Show("Please Select Account Type");
            }

            else if (string.IsNullOrWhiteSpace(comboBox4.Text))
            {
                MessageBox.Show("Please Select Group");
            }
            else if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please Select Ledger");
            }
            else
            {
                totalExpense = totalExpense + balance;
                textBox4.Text = totalExpense.ToString();
                dataGridView4.Rows.Add(Lid, textBox2.Text, balance, ExpenseSid, EGId);
                ClearExpenses();
                CalculatePNL();
            }
        }
    }
}
