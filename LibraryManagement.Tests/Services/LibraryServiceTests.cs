using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Data;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Services;
using LibraryManagement.Core.Exceptions;
using System.ComponentModel.DataAnnotations;
using Moq;

namespace LibraryManagement.Tests.Services;

public class LibraryServiceTests
{
    [Fact]
    public async Task GetMembersAsync_ReturnsAllMembers()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(options);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        context.Members.AddRange(
            new Member { MemberName = "Ali", PhoneNumber = "09123456789", IsActive = true },
            new Member { MemberName = "Sara", PhoneNumber = "09123456780", IsActive = true }
        );

        await context.SaveChangesAsync();

        var service = new LibraryService(context);

        // Act
        var result = await service.GetMembersAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Ali", result[0].MemberName);
        Assert.Equal("Sara", result[1].MemberName);
    }

    [Fact]
    public async Task GetMembersAsync_ReturnEmptyListWhenNoMembersExist()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var Context = new LibraryDbContext(option);

        await Context.Database.OpenConnectionAsync();
        await Context.Database.EnsureCreatedAsync();


        var service = new LibraryService(Context);

        //Act
        var result = await service.GetMembersAsync();

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindMemberByIdAsync_ReturnsMemberWhenExists()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member { Id = 1, MemberName = "Ali", PhoneNumber = "09122125589", IsActive = true };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service = new LibraryService(context);

        //Act
        var result = await service.FindMemberByIdAsync(1);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(member.Id, result.Id);
        Assert.Equal(member.MemberName, result.MemberName);

    }

    [Fact]
    public async Task FindMemberByIdAsync_ReturnNullWhenMemberDoesNotExist()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member { Id = 1, MemberName = "Ali", PhoneNumber = "09256589633", IsActive = true };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service = new LibraryService(context);

        //Act & Assert        
        await Assert.ThrowsAsync<MemberNotFoundException>(() => service.FindMemberByIdAsync(2));
    }

    [Fact]
    public async Task AddMemberAsync_SaveMemberToDatabase()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var service = new LibraryService(context);

        //Act
        await service.AddMemberAsync("Ali", "09123122664");

        //Assert

        var savedMember = await context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.PhoneNumber == "09123122664");

        Assert.NotNull(savedMember);
        Assert.Equal("Ali", savedMember.MemberName);
        Assert.True(savedMember.IsActive);
    }

    [Fact]
    public async Task UpdateMemberAsync_ThrowsArgumentException_WhenBothFieldsAreNull()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        var member = new Member
        {
            Id = 1,
            IsActive=true            
        };
        
        var service=new LibraryService(context);
              
        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(()=> service.UpdateMemberAsync(member));
    }

    [Fact]
    public async Task UpdateMemberAsync_UpdateOnlyMemberName()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context=new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member
        {
            Id = 1,
            MemberName = "Ali",
            PhoneNumber = "09123122664",
            IsActive = true
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service = new LibraryService(context);

        //Act
        var updatedData = new Member
        {
            Id = 1,
            MemberName = "Sara"
        };

        await service.UpdateMemberAsync(updatedData);

        //Assert
        var updatedMember = await context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id ==1);

        Assert.NotNull(updatedMember);
        Assert.Equal("Sara", updatedMember.MemberName);
        Assert.Equal(member.PhoneNumber,updatedMember.PhoneNumber);

    }

    [Fact]
    public async Task UpdateMemberAsync_UpdateOnlyPhoneNumber()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member
        {
            Id = 1,
            MemberName = "Ali",
            PhoneNumber = "09123122664",
            IsActive = true
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service = new LibraryService(context);

        //Act
        var updatedData = new Member
        {
            Id = 1,
            PhoneNumber="09121111111"
        };

        await service.UpdateMemberAsync(updatedData);

        //Assert
        var updatedMember = await context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == 1);

        Assert.NotNull(updatedMember);
        Assert.Equal("Ali", updatedMember.MemberName);
        Assert.Equal(updatedData.PhoneNumber, updatedMember.PhoneNumber);
    }


    [Fact]
    public async Task UpdateMemberAsync_UpdatesBothFields()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member
        {
            Id = 1,
            MemberName = "Ali",
            PhoneNumber = "09123122664",
            IsActive = true
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service = new LibraryService(context);

        //Act
        var updatedData = new Member
        {
            Id = 1,
            MemberName="Sara",
            PhoneNumber = "09121111111"
        };

        await service.UpdateMemberAsync(updatedData);

        //Assert
        var updatedMember = await context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == 1);

        Assert.NotNull(updatedMember);
        Assert.Equal(updatedData.MemberName, updatedMember.MemberName);
        Assert.Equal(updatedData.PhoneNumber, updatedMember.PhoneNumber);
    }

    [Fact]
    public async Task UpdateMemberAsync_ThrowsMemberNotFoundException_WhenMemberDoesNotExist()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context= new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member
        {
            Id = 100,
            MemberName = "Ali"
        };

        var service=new LibraryService(context);

        //Act & Assert       
        await Assert.ThrowsAsync<MemberNotFoundException>(() => service.UpdateMemberAsync(member));
    }

    [Fact]
    public async Task EnableMemberAsync_ActivateInactiveMember()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member
        {
            Id = 1,
            MemberName = "Ali",
            PhoneNumber = "09111111111",
            IsActive = false
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service=new LibraryService(context);

        //Act
        
        await service.EnableMemberAsync(member.Id);

        //Assert
        var updatedMember = await context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == member.Id);
        
        Assert.NotNull(updatedMember);
        Assert.True(updatedMember.IsActive);
    }

    [Fact]
    public async Task EnableMemberAsync_ThrowsMemberAlreadyActiveException_WhenMemberIsAlreadyActive()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context= new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member
        {
            Id = 1,
            MemberName = "Ali",
            PhoneNumber="09222222222",
            IsActive = true
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service= new LibraryService(context);

        //Act & Assert
        await Assert.ThrowsAsync<MemberAlreadyActiveException>(() => service.EnableMemberAsync(member.Id));
            
    }

    [Fact]
    public async Task DisableMemberAsync_DisableAvtiveMember()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context=new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member
        {
            Id = 1,
            MemberName = "Ali",
            PhoneNumber = "09123122664",
            IsActive = true
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service=new LibraryService(context);

        //Act
        await service.DisableMemberAsync(member.Id);

        //Assert
        var updatedMember=await context.Members.AsNoTracking().FirstOrDefaultAsync(m=> m.Id==member.Id);

        Assert.NotNull(updatedMember);
        Assert.False(updatedMember.IsActive);
    }


    [Fact]
    public async Task DisableMemberAsync_ThrowsMemberInactiveException_WhenMemberIsAlreadyInactive()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var member = new Member
        {
            Id = 1,
            MemberName = "Ali",
            PhoneNumber = "09222222222",
            IsActive = false
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var service = new LibraryService(context);

        //Act & Assert
        await Assert.ThrowsAsync<MemberInactiveException>(() => service.DisableMemberAsync(member.Id));
    }

    [Fact]
    public async Task GetBooksAsync_ReturnsAllBooks()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        context.Books.AddRange(
            new Book 
            {
                Id = 1, 
                Title = "بوف کور", 
                Author = "صادق هدایت", 
                PublishYear = 1342, 
                IsAvailable = true
            },

            new Book
            {
                Id = 2,
                Title = "شوهر آهو خانم",
                Author = "محمد افغانی",
                PublishYear = 1343,
                IsAvailable = true
            });

        await context.SaveChangesAsync();

        var service=new LibraryService(context);

        //Act
        var result= await service.GetBooksAsync();

        //Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
    }

    [Fact]
    public async Task GetBooksAsync_ReturnsEmptyListWhenNoBookExists()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:").Options;

        await using var context = new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var service = new LibraryService(context);

        //Act
        var result = await service.GetBooksAsync();

        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddBookAsync_SavesBookAndSetsAvailabilityToTrue()
    {
        //Arrange
        var option = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        await using var context=new LibraryDbContext(option);

        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var book = new Book
        {
            Id = 1,
            Title = "بوف کور",
            Author = "صادق هدایت",
            PublishYear = 1340,
            IsAvailable = false
        };        

        var service= new LibraryService(context);

        //Act
        await service.AddBookAsync(book);

        //Assert
        var result=await context.Books.AsNoTracking().FirstOrDefaultAsync(b =>  b.Id == book.Id);

        Assert.NotNull(result);
        Assert.True(result.IsAvailable);
    }

    [Fact]
    public void Welcome_CallSendWithCorrectMessage()
    {
        //Arrange
        var mockSender = new Mock<IMessageSender>();
        var service=new WelcomeService(mockSender.Object);
        //Act
        service.Welcome("Ali");
        //Assert
        mockSender.Verify(x => x.Send("Welcome Ali"), Times.Once());
    }


}