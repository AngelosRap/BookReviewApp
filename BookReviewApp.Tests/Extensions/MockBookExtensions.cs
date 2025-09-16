using BookReviewApp.Core.Interfaces;
using BookReviewApp.Core.Models;
using BookReviewApp.Domain.Models;
using Moq;

namespace BookReviewApp.Tests.Extensions;

public static class MockBookExtensions
{
    public static void SetupCreateBookSuccess(this Mock<IBookService> mock, Book book, string message) =>
        mock.Setup(s => s.Create(It.IsAny<Book>()))
            .ReturnsAsync(Result<Book>.CreateSuccessful(book, message));

    public static void SetupCreateBookFail(this Mock<IBookService> mock, string message) =>
        mock.Setup(s => s.Create(It.IsAny<Book>()))
            .ReturnsAsync(Result<Book>.CreateFailed(message));

    public static void SetupGetBookSuccess(this Mock<IBookService> mock, int id, string message) =>
    mock.Setup(s => s.Get(id))
        .ReturnsAsync(Result<Book>.CreateSuccessful(new Book { Id = id }, message));

    public static void SetupGetBookFail(this Mock<IBookService> mock, int id, string message) =>
        mock.Setup(s => s.Get(id))
            .ReturnsAsync(Result<Book>.CreateFailed(message));

    public static void SetupUpdateBookSuccess(this Mock<IBookService> mock, Book book, string message) =>
        mock.Setup(s => s.Update(It.IsAny<Book>()))
            .ReturnsAsync(Result<Book>.CreateSuccessful(book, message));

    public static void SetupUpdateBookFail(this Mock<IBookService> mock, string message) =>
        mock.Setup(s => s.Update(It.IsAny<Book>()))
            .ReturnsAsync(Result<Book>.CreateFailed(message));
}
