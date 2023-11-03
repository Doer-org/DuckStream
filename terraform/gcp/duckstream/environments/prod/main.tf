module "duckstream_api" {
  source          = "../../modules/api"
  project_id      = var.project_id
  default_region  = var.default_region
  service_name    = var.service_name
  container_image = var.container_image

  app_environment = var.app_environment
  app_db_name     = var.app_db_name
  app_db_user     = var.app_db_user
  app_db_password = var.app_db_password
  app_db_host     = var.app_db_host
  app_db_port     = var.app_db_port
  app_server_port = var.app_server_port
  app_gcs_url     = var.app_gcs_url
  app_gcs_bucket  = var.app_gcs_bucket
  app_gcs_key     = var.app_gcs_key
  app_ml_url      = var.app_ml_url
  app_ml_timeout  = var.app_ml_timeout
  app_client_url  = var.app_client_url
}

module "duckstream_storage" {
  source         = "../../modules/storage"
  bucket_name    = var.bucket_name
  default_region = var.default_region
}

module "iam" {
  source                = "../../modules/iam"
  bucket_name           = module.duckstream_storage.bucket_name
  service_account_email = var.iam_sa_email
}
