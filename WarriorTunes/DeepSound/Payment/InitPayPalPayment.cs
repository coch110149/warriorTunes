﻿using System;
using System.Linq;
using Android.App;
using Com.Braintreepayments.Api.Dropin;
using Com.Braintreepayments.Api.Dropin.Utils;
using Com.Braintreepayments.Api.Models;
using Com.Paypal.Android.Sdk.Onetouch.Core;
using DeepSound.Helpers.Utils;

namespace DeepSound.Payment
{
    public class InitPayPalPayment
    {
        private readonly Activity ActivityContext;
        public const int PayPalDataRequestCode = 7171;

        public InitPayPalPayment(Activity activity)
        {
            try
            {
                ActivityContext = activity;

                PayPalOneTouchCore.UseHardcodedConfig(activity, false);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Paypal
        public void BtnPaypalOnClick(string price)
        {
            try
            {
                var dropInRequest = InitPayPal(price);
                if (dropInRequest == null)
                    return;

                ActivityContext.StartActivityForResult(dropInRequest.GetIntent(ActivityContext), PayPalDataRequestCode);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private DropInRequest InitPayPal(string price)
        {
            try
            {
                var option = ListUtils.MyUserInfoList.FirstOrDefault();

                var currency = ListUtils.SettingsSiteList?.PaypalCurrency ?? "USD";

                //var googlePayment = new GooglePaymentRequest()
                //    .InvokeTransactionInfo(TransactionInfo.NewBuilder()
                //        .SetTotalPrice(price)
                //        .SetCurrencyCode(currency)
                //        .SetTotalPriceStatus(WalletConstants.TotalPriceStatusFinal)
                //        .Build())
                //    .PaypalEnabled(true)
                //    .EmailRequired(true);

                ThreeDSecurePostalAddress billingAddress = new ThreeDSecurePostalAddress()
                    .InvokeGivenName("Jill")
                    .InvokeSurname("Doe")
                    .InvokePhoneNumber("5551234567")
                    .InvokeStreetAddress("555 Smith St")
                    .InvokeExtendedAddress("#2")
                    .InvokeLocality("Chicago")
                    .InvokeRegion("IL")
                    .InvokePostalCode("12345")
                    .InvokeCountryCodeAlpha2("US");

                ThreeDSecureAdditionalInformation additionalInformation = new ThreeDSecureAdditionalInformation()
                    .InvokeAccountId("account-id");

                ThreeDSecureRequest threeDSecureRequest = new ThreeDSecureRequest()
                    .InvokeAmount(price)
                    .InvokeVersionRequested(ThreeDSecureRequest.Version2);

                if (option != null)
                    threeDSecureRequest.InvokeEmail(option.Email).InvokeMobilePhoneNumber(option.PhoneNumber);

                threeDSecureRequest
                    .InvokeBillingAddress(billingAddress)
                    .InvokeAdditionalInformation(additionalInformation);

                PayPalRequest paypalRequest = new PayPalRequest(price)
                    .InvokeCurrencyCode(currency)
                    .InvokeMerchantAccountId(AppSettings.MerchantAccountId)
                    .InvokeDisplayName(AppSettings.ApplicationName)
                    .InvokeBillingAgreementDescription("Pay the card")
                    //.InvokeLandingPageType("billing")
                    .InvokeIntent(PayPalRequest.IntentAuthorize);

                DropInRequest dropInRequest = new DropInRequest();
                //.ClientToken(MAuthorization)

                if (ListUtils.SettingsSiteList != null)
                {
                    switch (ListUtils.SettingsSiteList?.PaypalMode)
                    {
                        case "sandbox":
                            dropInRequest.TokenizationKey(AppSettings.SandboxTokenizationKey);
                            break;
                        case "live":
                            dropInRequest.TokenizationKey(AppSettings.ProductionTokenizationKey);
                            break;
                        default:
                            dropInRequest.TokenizationKey(AppSettings.ProductionTokenizationKey);
                            break;
                    }
                }

                dropInRequest.RequestThreeDSecureVerification(true);
                dropInRequest.CollectDeviceData(true);
                //dropInRequest.GooglePaymentRequest(getGooglePaymentRequest());
                dropInRequest.PaypalRequest(paypalRequest);
                dropInRequest.MaskCardNumber(true);
                dropInRequest.MaskSecurityCode(true);
                dropInRequest.AllowVaultCardOverride(false);
                dropInRequest.VaultCard(true);
                dropInRequest.VaultManager(false);
                //dropInRequest.InvokeCardholderNameStatus(CardForm.FieldDisabled);
                dropInRequest.InvokeThreeDSecureRequest(threeDSecureRequest);

                return dropInRequest;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public static void DisplayResult(PaymentMethodNonce paymentMethodNonce, string deviceData)
        {
            try
            {
                var mNonce = paymentMethodNonce;
                var mPaymentMethodType = PaymentMethodType.ForType(mNonce);

                var mPaymentMethodIcon = mPaymentMethodType.Drawable;
                var mPaymentMethodTitle = (paymentMethodNonce.TypeLabel);
                var mPaymentMethodDescription = (paymentMethodNonce.Description);

                var mNonceString = ": " + mNonce.Nonce;

                string details = "";
                if (mNonce is CardNonce cardNonce)
                {
                    details = "Card Last Two: " + cardNonce.LastTwo + "\n";
                    details += "3DS isLiabilityShifted: " + cardNonce.ThreeDSecureInfo.IsLiabilityShifted + "\n";
                    details += "3DS isLiabilityShiftPossible: " + cardNonce.ThreeDSecureInfo.IsLiabilityShiftPossible;
                }
                else if (mNonce is PayPalAccountNonce paypalAccountNonce)
                {
                    details = "First name: " + paypalAccountNonce.FirstName + "\n";
                    details += "Last name: " + paypalAccountNonce.LastName + "\n";
                    details += "Email: " + paypalAccountNonce.Email + "\n";
                    details += "Phone: " + paypalAccountNonce.Phone + "\n";
                    details += "Payer id: " + paypalAccountNonce.PayerId + "\n";
                    details += "Client metadata id: " + paypalAccountNonce.ClientMetadataId + "\n";
                    details += "Billing address: " + paypalAccountNonce.BillingAddress + "\n";
                    details += "Shipping address: " + paypalAccountNonce.ShippingAddress;
                }
                else if (mNonce is VenmoAccountNonce venmoAccountNonce)
                {
                    details = "Username: " + venmoAccountNonce.Username;
                }
                //else if (mNonce is GooglePaymentCardNonce googlePaymentCardNonce)
                //{
                //    details = "Underlying Card Last Two: " + googlePaymentCardNonce.LastTwo + "\n";
                //    details += "Email: " + googlePaymentCardNonce.Email + "\n";
                //    details += "Billing address: " + googlePaymentCardNonce.BillingAddress + "\n";
                //    details += "Shipping address: " + googlePaymentCardNonce.ShippingAddress;
                //}

                Console.WriteLine(details);
                Console.WriteLine("Device Data: " + deviceData);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
} 