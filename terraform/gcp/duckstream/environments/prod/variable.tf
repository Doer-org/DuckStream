variable "GOOGLE_CREDENTIALS" {
  description = "The Google Cloud credentials in JSON format"
  type        = string
}

variable "project_id" {
  description = "project id"
  type        = string
}

variable "default_region" {
  description = "The default region for resources"
  type        = string
}

variable "iam_sa_email" {
  description = "The email address of the service account"
  type        = string
}

variable "container_image" {
  description = "The container image to deploy"
  type        = string
}

variable "bucket_name" {
  description = "The name of the bucket"
  type        = string
}

variable "service_name" {
  description = "The name of the service"
  type        = string
}


# application envs
variable "app_environment" {
  description = "The environment to deploy to"
  type        = string
}

variable "app_db_name" {
  description = "The name of the database"
  type        = string
}

variable "app_db_user" {
  description = "The name of the database user"
  type        = string
}

variable "app_db_password" {
  description = "The password of the database user"
  type        = string
}

variable "app_db_host" {
  description = "The host of the database"
  type        = string
}

variable "app_db_port" {
  description = "The port of the database"
  type        = string
}

variable "app_server_port" {
  description = "The port of the server"
  type        = string
}

variable "app_gcs_url" {
  description = "The url of the gcs bucket"
  type        = string
}

variable "app_gcs_bucket" {
  description = "The name of the gcs bucket"
  type        = string
}

variable "app_gcs_key" {
  description = "The key of the gcs bucket"
  type        = string
}

variable "app_ml_url" {
  description = "The url of the ml service"
  type        = string
}

variable "app_ml_timeout" {
  description = "The timeout of the ml service"
  type        = string
}

variable "app_client_url" {
  description = "The url of the client"
  type        = string
}
