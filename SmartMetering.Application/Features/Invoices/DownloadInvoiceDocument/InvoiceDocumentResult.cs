namespace SmartMetering.Application.Features.Invoices.DownloadInvoiceDocument;

public record InvoiceDocumentResult(byte[] Content, string ContentType, string FileName);
