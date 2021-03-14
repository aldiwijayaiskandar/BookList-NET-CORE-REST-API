﻿using AutoMapper;
using Boilerplate.DomainLayer.Authors;
using Boilerplate.Helpers.Repository;
using Boilerplate.Models.Authors;
using Boilerplate.Models.Pagination;
using Boilerplate.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boilerplate.ApplicationLayer.Authors
{
    public class AuthorService : IAuthorService
    {
        private readonly IRepository<Author, Guid> _authorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IRepository<Author, Guid> authorRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AuthorPaginationDto> Find(int pageSize, int currentPage, Sort sort)
        {
            try
            {
                IEnumerable<Author> authors = await _authorRepository.FindAsync();
                int authorCount = authors.Count();

                return new AuthorPaginationDto()
                {
                    Total = authorCount,
                    Authors = _mapper.Map<IEnumerable<Author>, IEnumerable<AuthorDto>>(authors),
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw ErrorResponse.InternalServerError(ex);
            }
        }
        public async Task<AuthorDto> Add(CreateAuthorDto book)
        {
            try
            {
                Author author = Author.Create(book);

                await _authorRepository.AddAsync(author);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<Author, AuthorDto>(author);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw ErrorResponse.InternalServerError(ex);
            }
        }
        public async Task<AuthorDto> Update(Guid id, CreateAuthorDto updatedAuthor)
        {
            try
            {
                Author author = _mapper.Map<CreateAuthorDto, Author>(updatedAuthor);

                await _authorRepository.UpdateAsync(id, author);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<Author, AuthorDto>(author);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw ErrorResponse.InternalServerError(ex);
            }
        }
        public async Task<AuthorDto> Remove(Guid id)
        {
            try
            {
                Author author = _authorRepository.Remove(id);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<Author, AuthorDto>(author);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw ErrorResponse.InternalServerError(ex);
            }
        }
    }
}
