using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeetleX.FastHttpApi;
using iTextSharp.text;
using iTextSharp.text.pdf;
using zbwebcore.API.Config;
using zbwebcore.API.Model;

namespace zbwebcore.API.Controllers
{
    [Options(AllowOrigin = "*", AllowHeaders = "*,token")]
    [Controller(BaseUrl = "pdfdoc/")]
    public class PdfDocController
    {
        [Post]
        public object GetPdf(IHttpContext context)
        {
            bool flag = false;
            string msg = "生成PDF文件失败!", src = "";

            try
            {
                var yhbh = ConfigInfo.Info.getUser(context.Request.Header["token"]).yhbh;
                var yhmc= ConfigInfo.Info.getUser(context.Request.Header["token"]).yhmc;
                //创建 Document
                //Document pdfDoc = new Document(new Rectangle(100.7f, 140.7f), 600, 600, 600, 600);
                Document pdfDoc = new Document(PageSize.A4);
                //创建 PdfWriter 将文档放入内存流
                MemoryStream pdfStream = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream);
                pdfDoc.Open();
                //新的pdf页面
                pdfDoc.NewPage();
             
                BaseFont bfchinese = BaseFont.CreateFont(@"pdfdocprn\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Font ChFont = new Font(bfchinese, 10);
                Font ChFont14 = new Font(bfchinese, 18,Font.BOLD);
                Font ChFont_blue = new Font(bfchinese, 40, Font.NORMAL, new BaseColor(51, 0, 153));
                Font ChFont_msg = new Font(bfchinese, 12, Font.ITALIC, BaseColor.Red);
                //string fontsfolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts);
                //BaseFont bfChinese = BaseFont.CreateFont(bfchinese, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                //Font smallThreeFont = new Font(bfChinese, 15);
                //smallThreeFont.SetColor(47, 79, 79);


                // 设置列宽
                float[] columnWidths = { 0.8f, 0.8f, 0.5f, 0.5f };
                

                PdfPCell cell;

                string pstrtop = "|中百大厦收款收据||共1页,第1页|交款单位:000005-北京金宝银商贸股份有限责任公司集团公司|部门:06-营业六部(针纺羽绒)|分店:0401-胜利店|交费方式:04-支付宝";
                string pstrbutton = "此单一式三联:第一联收款单位记录 第二联缴款单位收执 第三联存根|打印单号:2040104051911158557|开票人:000057-张超人|打印日期:2020-01-01";
                string pstrbody = "费用项目|金额|费用项目|金额|收款单位盖章|汇款手续费|15.00|场地管理费|20.00|汇款手续费|15.00|场地管理费|20.00|汇款手续费|15.00|场地管理费|20.00|费用合计:|35.00|人民币大写:叁拾伍元整|扣款日期:2019-08-12";
                //PdfPTable tabletitle = new PdfPTable(new float[] { .5f, .9f, .5f });
                //tabletitle.WidthPercentage = 100; // 宽度100%填充
                //tabletitle.SetWidths(columnWidths);
                //表头
                PdfPTable tabletop = new PdfPTable(4);
                tabletop.WidthPercentage = 100; // 宽度100%填充
                //tabletop.SpacingBefore = 10f; // 前间距
                //tabletop.SpacingAfter = 10f; // 后间距
                tabletop.SetWidths(columnWidths);
                string[] arraytop = pstrtop.Split("|");
                for (int i = 0; i < arraytop.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(arraytop[i], (i==1? ChFont14:ChFont)));
                    if (arraytop[i].IndexOf("页") >= 0)
                    {
                        cell.HorizontalAlignment = Cell.ALIGN_RIGHT; // 设置水平居中
                        cell.VerticalAlignment = Cell.ALIGN_BOTTOM;
                    }
                    else
                    {
                        cell.HorizontalAlignment = Cell.ALIGN_BOTTOM; // 设置水平居中
                    }
                    
                    cell.Border = 0;

                    tabletop.AddCell(cell);
                }
                //表体
                PdfPTable tablebody = new PdfPTable(5);
                tablebody.WidthPercentage = 100; // 宽度100%填充
                //tablebutton.SpacingBefore = 10f; // 前间距
                //tablebutton.SpacingAfter = 10f; // 后间距
                //tablebody.SetWidths(columnWidths);
                string[] arraybody = pstrbody.Split("|");
                for (int i = 0; i < arraybody.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(arraybody[i], ChFont));
                    if (i <5)
                    {
                        cell.HorizontalAlignment = Cell.ALIGN_CENTER; // 设置水平居中
                        cell.VerticalAlignment = Cell.ALIGN_CENTER; // 设置垂直居中
                    }
                    
                    if (arraybody[i].IndexOf("人民币大写") >= 0)
                    {
                        cell.Colspan = 4;

                    }
                    if (arraybody[i].IndexOf("费用合计") >= 0)
                    {
                        cell.Colspan = 3;

                    }
                    if (arraybody[i].IndexOf("收款单位盖章") >= 0)
                    {
                        cell.Rowspan = 5;

                    }
                    tablebody.AddCell(cell);
                }
                //表尾
                PdfPTable tablebutton = new PdfPTable(4);
                tablebutton.WidthPercentage = 100; // 宽度100%填充
                //tablebutton.SpacingBefore = 10f; // 前间距
                //tablebutton.SpacingAfter = 10f; // 后间距
                tablebutton.SetWidths(columnWidths);
                string[] arraybutton = pstrbutton.Split("|");
                for (int i = 0; i < arraybutton.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(arraybutton[i], ChFont));
                    cell.Border = 0;
                    tablebutton.AddCell(cell);
                }

                pdfDoc.Add(tabletop);
                pdfDoc.Add(tablebody);
                pdfDoc.Add(tablebutton);
                pdfDoc.Add(new Paragraph(yhbh));
                pdfDoc.Add(new Paragraph(yhmc));
                pdfDoc.Add(new Paragraph("Hello World3!"));

                //关闭文档
                pdfDoc.Close();
                pdfWriter.Close();

                var retid = MongoHelper.UploadFileBytes("UploadFile", "testpdf", pdfStream.GetBuffer());
                if (!string.IsNullOrEmpty(retid.ToString()))
                {
                    src = "/upload/DownloadFilePDF?id=" + retid.ToString();
                    flag = true;
                    msg = "PDF文件生成成功!";

                    var mid = Dbservice.DB.Ado.GetString($"select fileid from qxgl_yhzd where yhbh='{yhbh}'");
                    if (!string.IsNullOrEmpty(mid))
                    {
                        
                        MongoHelper.DelFile("UploadFile", mid);
                    }
                    var retint=Dbservice.DB.Ado.ExecuteCommand($"update qxgl_yhzd set fileid='{retid.ToString()}' where yhbh='{yhbh}'");
                  
                }

            }
            catch (Exception ex)
            {
                msg = msg + ex.Message;
            }
            return new JsonResult(new { flag = flag, msg = msg, src = src });
        }

        //    public object Pdfsc(IHttpContext context)
        //    {
        //创建PdfPTable
        //PdfPTable table = new PdfPTable(new float[] {30f,20f, 25f, 25f });
        //table.WidthPercentage = 100;
        //table.DefaultCell.Padding = 0;
        //table.DefaultCell.UseAscender = true;
        ////carton .No
        //Chunk chunk = new Chunk("Carton No.");
        //PdfPCell cell = new PdfPCell(new Paragraph(chunk));
        //cell.BorderColor = BaseColor.Black; //添加table边框颜色
        //cell.Border = Rectangle.RIGHT_BORDER;//显示右边框
        //cell.BorderWidth = 2;
        //cell.PaddingLeft = 2;
        //cell.Colspan = 2;
        //cell.UseAscender = true;
        //table.AddCell(cell);
        //将table放在文档中
        //pdfDoc.Add(table);
        //        String planNameP = request.getParameter("planNameP");
        //        String ceritNameP = request.getParameter("ceritNameP");
        //        Date date = new Date();
        //        String startD = new SimpleDateFormat("HH-mm-ss").format(date);
        //        String oldPath = makePdfFilePath + pString + planNameP + "_" + ceritNameP + startD + ".pdf";
        //        try
        //        {
        //            /*String year = request.getParameter("year");
        //String cycle = request.getParameter("cycle");
        //String startDate = request.getParameter("startDate");
        //String endDate = request.getParameter("endDate");

        //Date startD1 = new SimpleDateFormat("yyyy-MM-dd").parse(startDate);
        //String startD = new SimpleDateFormat("yyyy年MM月dd日").format(startD1);
        //String start = new SimpleDateFormat("yyyy年MM月").format(startD1);

        //Date endD1 = new SimpleDateFormat("yyyy-MM-dd").parse(endDate);
        //String endD = new SimpleDateFormat("yyyy年MM月dd日").format(endD1);*/

        //            // 创建文件
        //            Document document = new Document();
        //            document.SetPageSize(PageSize.A4);
        //            // 建立一个书写器
        //            PdfWriter writer = PdfWriter.GetInstance(document, new FileOutputStream(oldPath));
        //            // 打开文件
        //            document.Open();

        //            // 中文字体,解决中文不能显示问题
        //            String FontChPath = PropertyUitls.getProperties("config.properties").getProperty("fontSourceSong");
        //            BaseFont bfChinese = BaseFont.CreateFont(FontChPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        //            // 蓝色字体
        //            Font blueFont = new Font(bfChinese);
        //            blueFont.SetColor(BaseColor.BLUE);
        //            blueFont.setSize(5);

        //            // 小三号
        //            Font smallThreeFont = new Font(bfChinese, 15);
        //            smallThreeFont.SetColor(BaseColor.BLACK);

        //            // 五号
        //            Font fiveFont = new Font(bfChinese);
        //            fiveFont.setColor(BaseColor.BLACK);
        //            fiveFont.setSize(10.5f);

        //            // 五号
        //            Font smallFiveFont = new Font(bfChinese);
        //            smallFiveFont.setColor(BaseColor.BLACK);
        //            smallFiveFont.setSize(9);

        //            // 五号
        //            Font smallSixFont = new Font(bfChinese);
        //            smallSixFont.setColor(BaseColor.BLACK);
        //            smallSixFont.setSize(6.5f);

        //            // 小四号 加粗
        //            Font greenFont = new Font(bfChinese, 12, Font.BOLD);
        //            greenFont.setColor(BaseColor.BLACK);

        //            // 小四号
        //            Font messFont = new Font(bfChinese, 12);
        //            messFont.setColor(BaseColor.BLACK);


        //            // 标题加粗 四号
        //            Font titleFont = new Font(bfChinese, 14, Font.BOLD);
        //            titleFont.setColor(BaseColor.BLACK);


        //            // 设计一个4列的表.
        //            PdfPTable table = new PdfPTable(4);
        //            table.SetWidthPercentage(100); // 宽度100%填充
        //            table.setSpacingBefore(10f); // 前间距
        //            table.setSpacingAfter(10f); // 后间距


        //            // 设置列宽
        //            float[] columnWidths = { 0.8f, 0.8f, 0.5f, 0.5f };
        //            table.SetWidths(columnWidths);
        //            PdfPCell cell;

        //            //第一行
        //            cell = new PdfPCell(new Phrase("", smallThreeFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorder(0);
        //            table.addCell(cell);

        //            cell = new PdfPCell(new Phrase("记    账    凭    证", smallThreeFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorderWidthTop(0);
        //            cell.setBorderWidthLeft(0);
        //            cell.setBorderWidthRight(0);
        //            table.addCell(cell);

        //            cell = new PdfPCell(new Phrase("本位币：CYN", fiveFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_RIGHT); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_BOTTOM); // 设置垂直居中
        //            cell.setBorder(0);
        //            cell.setColspan(2);
        //            table.addCell(cell);

        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setMinimumHeight(2); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorder(0);
        //            table.addCell(cell);

        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setMinimumHeight(2); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorderWidthTop(0);
        //            cell.setBorderWidthLeft(0);
        //            cell.setBorderWidthRight(0);
        //            table.addCell(cell);

        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setMinimumHeight(2); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorder(0);
        //            cell.setColspan(2);
        //            table.addCell(cell);

        //            //第二行

        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_LEFT); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorder(0);
        //            table.addCell(cell);

        //            cell = new PdfPCell(new Phrase("XXX公司职业年金计划", fiveFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorder(0);
        //            table.addCell(cell);


        //            cell = new PdfPCell(new Phrase("附单据数：0张", fiveFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_RIGHT); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setColspan(2);
        //            cell.setBorder(0);
        //            table.addCell(cell);

        //            //第三行

        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorder(0);
        //            table.addCell(cell);

        //            cell = new PdfPCell(new Phrase("凭证日期：XXXX年XX月XX日", fiveFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorder(0);
        //            table.addCell(cell);

        //            cell = new PdfPCell(new Phrase("凭证编号：XXXXX号", fiveFont));
        //            cell.setMinimumHeight(20); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_RIGHT); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell.setBorder(0);
        //            cell.setColspan(2);
        //            table.addCell(cell);

        //            // 设计一个4列的表.
        //            PdfPTable table0 = new PdfPTable(2);
        //            table0.setWidthPercentage(100); // 宽度100%填充


        //            // 设置列宽
        //            float[] columnWidths0 = { 0.8f, 0.8f };
        //            table0.setWidths(columnWidths0);
        //            PdfPCell cell0;

        //            //正文第1行
        //            cell0 = new PdfPCell(new Phrase("摘要", fiveFont));
        //            cell0.setMinimumHeight(28); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell0.setRowspan(2);
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("科目", fiveFont));
        //            cell0.setMinimumHeight(28); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            cell0.setRowspan(2);
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("{2018-11-24}税金计提", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("其他应付款-支付与转出", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            //2222222222
        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);

        //            cell0 = new PdfPCell(new Phrase("3", smallFiveFont));
        //            cell0.setMinimumHeight(20); // 设置单元格高度
        //            cell0.setUseAscender(true); // 设置可以居中
        //            cell0.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell0.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table0.addCell(cell0);




        //            //左上角
        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setMinimumHeight(150); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //                                                          //	            cell.enableBorderSide(1);
        //            cell.setRowspan(8);
        //            cell.setColspan(2);
        //            cell.setBorderWidthRight(0);
        //            cell.setBorderWidthBottom(0);
        //            cell.setPaddingRight(-0.2f);
        //            cell.addElement(table0);
        //            table.addCell(cell);

        //            // 借方发生的  设计一个10列的表.
        //            PdfPTable table1 = new PdfPTable(10);
        //            table1.setWidthPercentage(102); // 宽度100%填充
        //                                            //	            table1.setSpacingBefore(10f); // 前间距
        //                                            //	            table1.setSpacingAfter(10f); // 后间距

        //            // 设置列宽
        //            float[] columnWidths1 = { 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f };
        //            table1.setWidths(columnWidths1);
        //            PdfPCell cell1;

        //            cell1 = new PdfPCell(new Phrase("借方发生", fiveFont));
        //            cell1.setMinimumHeight(13); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_TOP); // 设置垂直居中
        //            cell1.setColspan(10);
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("千", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("百", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("十", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("万", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("千", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("百", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("十", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("元", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("角", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("分", smallSixFont));
        //            cell1.setMinimumHeight(15); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            //借方贷方的金额十列  第一行
        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            //第二行
        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            //第三行
        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            //第四行
        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            //第五行
        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            //第六行
        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            cell1 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell1.setMinimumHeight(20); // 设置单元格高度
        //            cell1.setUseAscender(true); // 设置可以居中
        //            cell1.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell1.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table1.addCell(cell1);

        //            //借方7行
        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setMinimumHeight(150); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_TOP); // 设置垂直居中
        //            cell.setBorderWidthRight(0);
        //            cell.setBorderWidthLeft(0);
        //            cell.setBorderWidthBottom(0);
        //            cell.setPaddingLeft(2.5f);
        //            cell.setPaddingBottom(-3);
        //            cell.setRowspan(8);
        //            cell.addElement(table1);
        //            table.addCell(cell);


        //            //贷方发生  
        //            // 设计一个10列的表.
        //            PdfPTable table2 = new PdfPTable(10);
        //            table2.setWidthPercentage(102); // 宽度100%填充

        //            // 设置列宽
        //            float[] columnWidths2 = { 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f };
        //            table2.setWidths(columnWidths2);
        //            PdfPCell cell2;

        //            cell2 = new PdfPCell(new Phrase("贷方发生", fiveFont));
        //            cell2.setMinimumHeight(13); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_TOP); // 设置垂直居中
        //            cell2.setColspan(10);
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("千", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("百", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("十", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("万", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("千", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("百", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("十", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("元", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("角", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("分", smallSixFont));
        //            cell2.setMinimumHeight(15); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            //借方贷方的金额十列  第一行
        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            //第二行
        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            //第三行
        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            //第四行
        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            //第五行
        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            //第六行
        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            cell2 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell2.setMinimumHeight(20); // 设置单元格高度
        //            cell2.setUseAscender(true); // 设置可以居中
        //            cell2.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell2.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table2.addCell(cell2);

        //            //贷方7行
        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setMinimumHeight(150); // 设置单元格高度
        //            cell.setUseAscender(true); // 设置可以居中
        //            cell.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell.setVerticalAlignment(Cell.ALIGN_TOP); // 设置垂直居中
        //            cell.setBorderWidthLeft(0);
        //            cell.setBorderWidthBottom(0);
        //            cell.setPaddingRight(3);
        //            cell.setPaddingBottom(-3);
        //            cell.setRowspan(8);
        //            cell.addElement(table2);
        //            table.addCell(cell);

        //            // 设计一个10列的表.
        //            PdfPTable table4 = new PdfPTable(1);
        //            table4.setWidthPercentage(100); // 宽度100%填充

        //            // 设置列宽
        //            float[] columnWidths4 = { 1.6f };
        //            table4.setWidths(columnWidths4);
        //            PdfPCell cell4;

        //            cell4 = new PdfPCell(new Phrase("金额合计：捌佰陆拾壹元陆角玖分", fiveFont));
        //            cell4.setMinimumHeight(20); // 设置单元格高度
        //            cell4.setUseAscender(true); // 设置可以居中
        //            cell4.setHorizontalAlignment(Cell.ALIGN_LEFT); // 设置水平居中
        //            cell4.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table4.addCell(cell4);

        //            //最后一行
        //            cell = new PdfPCell(new Phrase("", fiveFont));
        //            cell.setBorderWidthRight(0);
        //            cell.setBorderWidthTop(0);
        //            cell.setPaddingRight(-0.2f);
        //            cell.setPaddingTop(-0.5f);
        //            cell.addElement(table4);
        //            cell.setColspan(2);
        //            table.addCell(cell);

        //            // 设计一个10列的表.
        //            PdfPTable table3 = new PdfPTable(10);
        //            table3.setWidthPercentage(102); // 宽度100%填充

        //            // 设置列宽
        //            float[] columnWidths3 = { 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f };
        //            table3.setWidths(columnWidths3);
        //            PdfPCell cell3;

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            cell3 = new PdfPCell(new Phrase("", smallSixFont));
        //            cell3.setMinimumHeight(20); // 设置单元格高度
        //            cell3.setUseAscender(true); // 设置可以居中
        //            cell3.setHorizontalAlignment(Cell.ALIGN_CENTER); // 设置水平居中
        //            cell3.setVerticalAlignment(Cell.ALIGN_MIDDLE); // 设置垂直居中
        //            table3.addCell(cell3);

        //            //借方
        //            cell = new PdfPCell(new Phrase("", smallFiveFont));
        //            cell.setBorderWidthRight(0);
        //            cell.setBorderWidthLeft(0);
        //            cell.setBorderWidthTop(0);
        //            cell.setPaddingLeft(2.5f);
        //            cell.setPaddingTop(-0.5f);
        //            cell.addElement(table3);
        //            table.addCell(cell);

        //            //贷方
        //            cell = new PdfPCell(new Phrase("", smallFiveFont));
        //            cell.setBorderWidthLeft(0);
        //            cell.setBorderWidthTop(0);
        //            cell.setPaddingTop(-0.5f);
        //            cell.setPaddingRight(3);
        //            cell.addElement(table3);
        //            table.addCell(cell);

        //            document.add(table);

        //            // 表尾
        //            Paragraph thrtionTitle = new Paragraph("制单：系统运维人员     审核：               记账：\n\n", fiveFont);
        //            thrtionTitle.setLeading(10);
        //            thrtionTitle.setAlignment(Element.ALIGN_LEFT);
        //            document.add(thrtionTitle);

        //            // 关闭文档
        //            document.close();
        //            // 关闭书写器
        //            writer.close();
        //        }
        //        catch (Exception e)
        //        {
        //            e.printStackTrace();
        //            throw new Exception();
        //        }
        //        return oldPath;


        //    }
    }
}