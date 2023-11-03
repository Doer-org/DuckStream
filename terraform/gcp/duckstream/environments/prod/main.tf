module "duckstream_api" {
  source          = "../../modules/api"
  project_id      = var.project_id
  default_region  = var.default_region
  service_name    = var.service_name
  container_image = var.container_image
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
