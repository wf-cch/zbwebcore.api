using BeetleX.Buffers;
using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Config
{

    public class DownLoad : IResult
    {
        public DownLoad(string text)
        {
            Text = text;
        }
        public string Text { get; set; }
        public IHeaderItem ContentType => ContentTypes.OCTET_STREAM;
        public int Length { get; set; }
        public bool HasBody => true;
        public void Setting(HttpResponse response)
        {
            response.Header.Add("Content-Disposition", "attachment;filename=TcpBanchmarks.json");
        }
        public void Write(PipeStream stream, HttpResponse response)
        {
            stream.Write(Text);
        }
    }
    public class DownLoadExcel1 : IResult
    {
        public DownLoadExcel1(Stream _stream)
        {
            Stream = _stream;
        }
        public Stream Stream { get; set; }
        public IHeaderItem ContentType => ContentTypes.OCTET_STREAM;
        public int Length { get; set; }
        public bool HasBody => true;
        public void Setting(HttpResponse response)
        {
            response.Header.Add("Content-Disposition", "attachment;filename=download.xls");
            //response.Result(Stream);
        }

        public void Write(PipeStream stream, HttpResponse response)
        {
            //stream.Write(Stream.);
            //using (stream.LockFree())
            //{
            //    stream.Write(stream.ReadByte());
            //    response.Result(Stream);

            //}
        }
    }
    public class DownLoadExcel : IResult
    {
        public DownLoadExcel(byte[] _byte,string title)
        {
            _Byte = _byte;
            Title = title;
        }
        public byte[] _Byte { get; set; }
        public IHeaderItem ContentType => ContentTypes.X_XLS;
        public int Length { get; set; }
        public string Title { get; set; }
        public bool HasBody => true;
        public void Setting(HttpResponse response)
        {
            response.Header.Add("Content-Disposition", "attachment;filename="+(string.IsNullOrEmpty(Title) ?"DownloadFile": Title) +".xlsx");
        }

        public void Write(PipeStream stream, HttpResponse response)
        {
            stream.Write(_Byte);

        }
    }
    public class DownLoadPdf : IResult
    {
        public DownLoadPdf(byte[] _byte, string title)
        {
            _Byte = _byte;
            Title = title;
        }
        public byte[] _Byte { get; set; }
        public IHeaderItem ContentType => ContentTypes.PDF;
        public int Length { get; set; }
        public string Title { get; set; }
        public bool HasBody => true;
        public void Setting(HttpResponse response)
        {
            //response.Header.Add("ContentType", "application/pdf");
            response.Header.Add("Content-Disposition", "inline;filename=" + (string.IsNullOrEmpty(Title) ? "DownloadFile" : Title) + ".pdf");

        }

        public void Write(PipeStream stream, HttpResponse response)
        {
            stream.Write(_Byte);

        }
    }
    public class DownloadImg : IResult
    {
        public DownloadImg(byte[] _byte)
        {
            _Byte = _byte;
        }
        public byte[] _Byte { get; set; }
        public IHeaderItem ContentType => ContentTypes.OCTET_STREAM;
        public int Length { get; set; }
        public bool HasBody => true;
        public void Setting(HttpResponse response)
        {
            response.Header.Add("Content-Disposition", "attachment;");
        }

        public void Write(PipeStream stream, HttpResponse response)
        {
            stream.Write(_Byte);

        }
    }


}

