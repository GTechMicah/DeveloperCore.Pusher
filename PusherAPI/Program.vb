Imports System.Threading
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Scalar.AspNetCore

Public Module Program
    Public Sub Main(args As String())
        Dim builder As WebApplicationBuilder = WebApplication.CreateBuilder(args)
        Dim keys = builder.Configuration.GetSection("Keys").Get(Of List(Of String))()
        NotificationService.Keys = New HashSet(Of String)(keys)
        builder.Services.AddControllers()
        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddOpenApi()
        builder.Services.AddSingleton(Of NotificationService)
        Dim app As WebApplication = builder.Build()
        app.UseAuthorization()
        app.MapControllers()
        app.UseWebSockets()
        app.MapGet("/", Function() "yo were in")
        app.MapGet("/sse/{key}", Function(cancellationToken As CancellationToken, key As String)
            Return NotificationService.SseHandler(key, cancellationToken)
        End Function)
        app.Map("/ws", AddressOf NotificationService.SocketHandler)
        app.MapOpenApi()
        app.MapScalarApiReference()
#If DEBUG Then
        app.Run("http://localhost:7166")
#Else
        app.Run()
#End If
    End Sub
End Module