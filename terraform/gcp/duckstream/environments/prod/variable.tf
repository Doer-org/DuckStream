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
