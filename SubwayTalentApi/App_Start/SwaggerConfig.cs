using System.Web.Http;
using WebActivatorEx;
using SubwayTalentApi;
using Swashbuckle.Application;
using System;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Configuration;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace SubwayTalentApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "SubwayTalentApi");
                        c.IncludeXmlComments(GetXmlCommentsPath());
                        c.OperationFilter<AddFileParamTypes>();
                    })
                .EnableSwaggerUi(c =>
                    {

                    });

        }
        private static string GetXmlCommentsPath()
        {
            return String.Format(@"{0}\bin\SubwayTalentApi.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }

    }

    public class AddFileParamTypes : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry dataTypeRegistry, ApiDescription apiDescription)
        {

            var allowedMimeTypes = ConfigurationManager.AppSettings["ImageMimeTypes"] + "," + ConfigurationManager.AppSettings["VideoMimeTypes"];

            #region "Media_UploadMedia"
            if (operation.operationId == "Media_UploadMedia")  // controller and action name
            {
                operation.consumes.Add("multipart/form-data");
                operation.parameters = new List<Parameter>
                {
                    new Parameter
                    {
                        name = "UserId",
                        required = true,
                         @in = "Content-Header",
                        type = "string",
                         description = "UserId of the user you want to add media."
                    },
                    new Parameter
                    {
                        name = "file",
                        required = true,
                        type = "FileData",
                        description = "The paramter name doesn't matter. For now you can only send these mime types [ " + allowedMimeTypes  + "]. Do not use the Try it out! button below, it won't work."
                    },                    
                };
            }
            #endregion
           
            #region "Accounts_UpdateUserMultiPart"

            if (operation.operationId == "Accounts_UpdateUserMultiPart")  // controller and action name
            {
                operation.consumes.Add("multipart/form-data");
                operation.parameters = new List<Parameter>
                {
                    new Parameter
                    {
                        name = "UserId",
                        required = true,
                         @in = "Content-Header",
                        type = "string",
                         description = "UserId of the user you want to update."
                    },

                    new Parameter
                    {
                        name = "Bio",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "The short biography of the user."
                    },

                    new Parameter
                    {
                        name = "Birthday",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "City",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Email",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "FirstName",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "LastName",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Gender",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "Accepted values are M or F."
                    },

                    new Parameter
                    {
                        name = "MobileNumber",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "it has 15 max characters."
                    },

                    new Parameter
                    {
                        name = "Rate",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "Talent's rate per/hr."
                    },

                    new Parameter
                    {
                        name = "State",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "TalentName",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Skills[]",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "Values here should only be skills Id's. "
                    },

                    
                    new Parameter
                    {
                        name = "Genre[]",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "Values here should only be genre Id's."
                    }, 
                 
                     new Parameter
                    {
                        name = "ProfilePicTalent",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "the full path of talent's profile pic."
                    }, 

                     new Parameter
                    {
                        name = "ProfilePic",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "the full path of planner's profile pic."
                    }, 

                      new Parameter
                    {
                        name = "Perspective",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "values are  P- planner and T- talent"
                    }, 


                    new Parameter
                    {
                        name = "file",
                        required = true,
                        type = "FileData",
                        description = "The paramter name doesn't matter. For now you can only send these mime types [ " + allowedMimeTypes  + "]. Do not use the Try it out! button below, it won't work."
                    },                    
                };
            }
#endregion

            #region "Events_UpdateEventMultiPart"

            if (operation.operationId == "Events_UpdateEventMultiPart")  // controller and action name
            {
                operation.consumes.Add("multipart/form-data");
                operation.parameters = new List<Parameter>
                {
                    new Parameter
                    {
                        name = "Id",
                        required = true,
                         @in = "FormDataKey",
                        type = "integer",
                         description = "ID of the event."
                    },

                    new Parameter
                    {
                        name = "DateCreated",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "DateEnd",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "DateStart",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Description",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Location",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Name",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Status",
                        required = true,
                         @in = "FormDataKey",
                        type = "integer",
                         description = " values are 1 - confirmed, 0 - cancelled"
                    },

                    new Parameter
                    {
                        name = "Title",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Type[Id]",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = "Just input the type id of the event."
                    },

                    new Parameter
                    {
                        name = "Latitude",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },

                    new Parameter
                    {
                        name = "Longitude",
                        required = true,
                         @in = "FormDataKey",
                        type = "string",
                         description = ""
                    },
                    
                    new Parameter
                    {
                        name = "ProfilePic",
                        required = true,
                        type = "FileData",
                        description = "The paramter name doesn't matter. For now you can only send these mime types [ " + allowedMimeTypes  + "]. Do not use the Try it out! button below, it won't work."
                    },                    
                };
            }
            #endregion
        }
    }
}
