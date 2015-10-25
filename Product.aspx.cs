using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Product : System.Web.UI.Page
{
    SqlConnection conn = new SqlConnection("Data Source=ETL\\SQLEXPRESS;Initial Catalog=Product;Integrated Security=True;Pooling=False");
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd;
    SqlDataReader dr;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {

            cmd = new SqlCommand("SELECT DISTINCT PCatagory from tb_emp ", conn);

            conn.Open();
            dr = cmd.ExecuteReader();
            productDropDownList.Items.Add("Select");
            //proDropDownList.Items.Add("Select");


            while (dr.Read())
            {
                productDropDownList.Items.Add(dr.GetValue(0).ToString());
                //proDropDownList.Items.Add(dr.GetValue(0).ToString());

            }
            dr.Close();
            conn.Close();
            DataTable dt = new DataTable();
            dt.Columns.Add("PID", typeof(int));
            dt.Columns.Add("Details", typeof(String));
            dt.Columns.Add("Price", typeof(float));
            dt.Columns.Add("Itemno", typeof(int));
            dt.Columns.Add("tprice", typeof(float));
            Session["cart"] = dt;




        }
    }


    protected void productDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (productDropDownList.SelectedIndex > 0)
        {

            cmd = new SqlCommand("SELECT DISTINCT PType from tb_emp where PCatagory=@pcat", conn);
            string p = proDropDownList.Text;
            string s = productDropDownList.Text;
            cmd.Parameters.AddWithValue("@pcat", s);
            conn.Open();

            dr = cmd.ExecuteReader();
            //productDropDownList.Items.Add("Select");
            //proDropDownList.Items.Add("Select");
            proDropDownList.Items.Clear();
            proDropDownList.Items.Add("Select");
            while (dr.Read())
            {
                //productDropDownList.Items.Add(dr.GetValue(0).ToString());
                proDropDownList.Items.Add(dr.GetValue(0).ToString());

            }
            dr.Close();
            conn.Close();
            //BindData();
        }
    }
    protected void GetData()
    {
        string dno = productDropDownList.SelectedItem.ToString();
        string d = proDropDownList.SelectedItem.ToString();

        da = new SqlDataAdapter("SELECT PID,BrandName,Price,Stock,Details from tb_emp  where PType='" + d + "' AND PCatagory='" + dno + "'", conn);
        ds = new DataSet();
        da.Fill(ds, "localEmployee");
        Session["ds"] = ds;
        SqlCommandBuilder scb = new SqlCommandBuilder(da);
        Session["da"] = da;

    }
    protected void BindData()
    {
        DataTable dt = ((DataSet)Session["ds"]).Tables[0];
        productGridView.DataSource = ds.Tables[0];
        productGridView.DataBind();
    }
    protected void showButton_Click(object sender, EventArgs e)
    {

        GetData();
        BindData();


    }
    protected void productGridView_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (proDropDownList.SelectedIndex > 0)
        //{
        //    GetData();
        //    BindData();

        //}
        //else
        //{
        //    productGridView.DataSource = null;
        //    productGridView.DataBind();
        //}
       
    }
    protected void proDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //proDropDownList.Items.Clear();


    }

    public int PID;
    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        for (int i = 0; i < productGridView.Rows.Count; i++)
        {
            CheckBox ch = (CheckBox)productGridView.Rows[i].FindControl("CheckBox1");
            if (ch.Checked == true)
            {
                Label l = (Label)productGridView.Rows[i].FindControl("Label1");
                PID = int.Parse(l.Text);
                DropDownList dl = (DropDownList)productGridView.Rows[i].FindControl("DropDownList1");
                dl.Enabled = true;
            }
            else
            {
                DropDownList dl = (DropDownList)productGridView.Rows[i].FindControl("DropDownList1");
                dl.Enabled = false;

            }
        }
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void cartGridView_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)Session["cart"];

        for (int i = 0; i < productGridView.Rows.Count; i++)
        {
            CheckBox ch = (CheckBox)productGridView.Rows[i].FindControl("CheckBox1");
            float tp = 0;
            if (ch.Checked == true)
            {
                Label l = (Label)productGridView.Rows[i].FindControl("Label1");
                Label l1 = (Label)productGridView.Rows[i].FindControl("Label2");//price
                Label l2 = (Label)productGridView.Rows[i].FindControl("Label3");//details

                DropDownList dl = (DropDownList)productGridView.Rows[i].FindControl("DropDownList1");
                tp = float.Parse(l1.Text) * int.Parse(dl.SelectedItem.ToString());
                DataRow dr = dt.NewRow();
                dr[0] = int.Parse(l.Text);
                dr[1] = l2.Text;
                dr[2] = float.Parse(l1.Text);
                dr[3] = int.Parse(dl.SelectedItem.ToString());
                dr[4] = tp;
                dt.Rows.Add(dr);

            }
        }
        cartGridView.DataSource = dt;

        cartGridView.DataBind();
       
    }
    protected void cartGridView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DataTable dt = (DataTable)Session["cart"];
        int row = e.NewEditIndex;
        cartGridView.DataSource = dt;
        cartGridView.EditIndex = row;
        cartGridView.DataBind();

    }
    protected void cartGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        DataTable dt = (DataTable)Session["cart"];
        cartGridView.DataSource = dt;
        cartGridView.EditIndex = -1;
        cartGridView.DataBind();

    }
    protected void cartGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int row = e.RowIndex;
        DataTable dt = (DataTable)Session["cart"];
        TextBox t1 = (TextBox)cartGridView.Rows[row].FindControl("TextBox4");
        TextBox t2 = (TextBox)cartGridView.Rows[row].FindControl("TextBox3");
        int item = int.Parse(t1.Text);
        float p = int.Parse(t2.Text);
        float tp = item * p;
        dt.Rows[row][3] = item;
        dt.Rows[row][4] =tp ;
        cartGridView.DataSource = dt;
        cartGridView.EditIndex = -1;
        cartGridView.DataBind();
    }
    protected void cartGridView_SelectedIndexChanged1(object sender, EventArgs e)
    {

    }
}