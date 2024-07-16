using GHotel.Application.Exceptions;
using GHotel.Application.Models.Payment;
using GHotel.Application.UOW;
using GHotel.Application.Utilities;
using GHotel.Domain.Entities;
using GHotel.Domain.Enums;
using Mapster;

namespace GHotel.Application.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly IPaymentUtility _paymentUtility;
    private readonly IPaymentUnitOfWork _paymentUnitOfWork;

    public PaymentService(IPaymentUtility paymentUtility, IPaymentUnitOfWork paymentUnitOfWork)
    {
        _paymentUtility = paymentUtility;
        _paymentUnitOfWork = paymentUnitOfWork;
    }

    public async Task<PaymentResponseModel> CreatePayment(PaymentRequestModel paymentRequestModel, CancellationToken cancellationToken)
    {
        var newPayment = await _paymentUtility.CreatePayment(paymentRequestModel.Amount, paymentRequestModel.Currency,cancellationToken).ConfigureAwait(false);
        var payment = paymentRequestModel.Adapt<Payment>();
        payment.Id = newPayment.PaymentId;

        var result = await _paymentUnitOfWork.PaymentRepository.Add(payment, cancellationToken).ConfigureAwait(false);
        await _paymentUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var payemntResponseModel = result.Adapt<PaymentResponseModel>();
        payemntResponseModel.ApproveLink = newPayment.ApproveLink;

        return payemntResponseModel;
    }

    public async Task<PaymentResponseModel> CheckoutAuthorizePayment(string id,CancellationToken cancellationToken)
    {
        var targetPayment = await _paymentUnitOfWork.PaymentRepository.Get(id, cancellationToken).ConfigureAwait(false);
        if(targetPayment == null)
            throw new NotFoundException($"Payment with id: {id} was not found","PaymentNotFound");
        if(targetPayment.Status != PaymentStatuss.Pending)
            throw new InvalidOperationException($"Payment with id: {id} is not in pending state");
        targetPayment.AuthorizationId = await _paymentUtility.CheckoutAuthorizedPayment(targetPayment.Id, cancellationToken).ConfigureAwait(false);
        await _paymentUnitOfWork.PaymentRepository.Update(targetPayment, cancellationToken).ConfigureAwait(false);
        await _paymentUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return targetPayment.Adapt<PaymentResponseModel>();
    }

    public async Task<PaymentResponseModel> CaptureAuthorizePayment(string id,CancellationToken cancellationToken)
    {
        var targetPayemnt = await _paymentUnitOfWork.PaymentRepository.Get(id, cancellationToken).ConfigureAwait(false);
        if(targetPayemnt == null)
            throw new NotFoundException($"Payment with id: {id} was not found","PaymentNotFound");
        if(targetPayemnt.Status != PaymentStatuss.Pending)
            throw new InvalidOperationException($"Payment with id: {id} is not in pending state");
        if(targetPayemnt.AuthorizationId == null)
            throw new InvalidOperationException($"Payment with id: {id} is not authorized");

        await _paymentUtility.CaptureAuthorizePayment(targetPayemnt.AuthorizationId,targetPayemnt.Amount,targetPayemnt.Currency, cancellationToken).ConfigureAwait(false);
        targetPayemnt.Status = PaymentStatuss.Paid;
        await _paymentUnitOfWork.PaymentRepository.Update(targetPayemnt, cancellationToken).ConfigureAwait(false);

        return targetPayemnt.Adapt<PaymentResponseModel>();
    }

    public async Task<PaymentResponseModel> CancelPayment(string id, decimal fee, CancellationToken cancellationToken)
    {
        var targetPayment = await _paymentUnitOfWork.PaymentRepository.Get(id, cancellationToken).ConfigureAwait(false);
        if(targetPayment == null)
            throw new NotFoundException($"Payment with id: {id} was not found","PaymentNotFound");
        if(targetPayment.Status != PaymentStatuss.Pending)
            throw new InvalidOperationException($"Payment with id: {id} is not in pending state");
        if(fee != 0)
            await _paymentUtility.CaptureAuthorizePayment(targetPayment.AuthorizationId!,fee,targetPayment.Currency,cancellationToken).ConfigureAwait(false);

        targetPayment.Status = PaymentStatuss.Cancelled;
        await _paymentUnitOfWork.PaymentRepository.Update(targetPayment, cancellationToken).ConfigureAwait(false);
        await _paymentUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return targetPayment.Adapt<PaymentResponseModel>();
    }

}
