What EnterpriseLibrary.Logging.Serilog does
============================================

If you use EnterpriseLibrary and you'd like to send your logs through Serilog,
you can use this library to plumb the two logging systems together.

How to use this
===================

In your `App.config` or `Web.config`, locate the `loggingConfiguration` section 

1. add the listener

2. link the listener to the required categories

        <listeners>
          <add 
            name="SerilogTraceListener1" 
            type="EnterpriseLibrary.Logging.Serilog.SerilogTraceListener, EnterpriseLibrary.Logging.Serilog" 
            listenerDataType="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.CustomTraceListenerData, Microsoft.Practices.EnterpriseLibrary.Logging, Version=5.0.505.1, Culture=neutral" />
        </listeners >
        <categorySources>
          <add switchValue="All" name="General">
            <listeners>
              <add name= "SerilogTraceListener1" />
            </listeners>
          </add>
        </categorySources>
